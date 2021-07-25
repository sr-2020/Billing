using Billing;
using Billing.Dto;
using Billing.Dto.Shop;
using Billing.Services;
using Core;
using Core.Model;
using Core.Primitives;
using InternalServices;
using IoC;
using Scoringspace;
using Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public class JobLifeService : BaseService
    {
        int _bulk = 1;
        public JobLifeService() : base()
        {
            _bulk = Factory.Settings.GetIntValue(SystemSettingsEnum.parallel_count);
        }
        public const string BillingException = "Биллинг заблокирован";

        public string ToggleCycle(string token = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                token = Factory.Job.GetCurrentToken();
            }
            var cycle = Factory.Job.GetLastCycle(token);

            if (cycle.IsActive == false)
            {
                cycle = new BillingCycle
                {
                    Number = cycle.Number
                };
                cycle.Number++;
            }
            cycle.IsActive = !cycle.IsActive;
            cycle.Token = token;
            Factory.Job.AddAndSave(cycle);
            return $"Цикл {cycle.Token}_{cycle.Number} {(cycle.IsActive ? "стартовал" : "остановлен")}";
        }

        public string DoBeat(BeatTypes type = BeatTypes.Test, string token = "", bool wait = false)
        {
            var cycle = Factory.Job.GetLastCycle(token);
            if (!cycle.IsActive || cycle == null)
            {
                return "цикл не запущен";
            }
            if (type == BeatTypes.Characters)
            {
                if (!Factory.Job.BlockBilling())
                {
                    throw new BillingException(BillingException);
                }
            }
            var task = Task.Run(() =>
            {
                try
                {
                    var beat = Factory.Job.GetLastBeatAsNoTracking(cycle.Id, type);
                    var newBeat = new BillingBeat();
                    newBeat.Number = beat != null ? beat.Number + 1 : 1;
                    newBeat.StartTime = DateTime.Now.ToUniversalTime();
                    newBeat.CycleId = cycle.Id;
                    newBeat.BeatType = (int)type;
                    Factory.Job.AddAndSave(newBeat);
                    var dto = new JobLifeDto
                    {
                        Beat = newBeat
                    };
                    switch ((BeatTypes)newBeat.BeatType)
                    {
                        case BeatTypes.Test:
                            dto.AddHistory("test beat");
                            break;
                        case BeatTypes.Items:
                            dto = DoItemsBeat(dto);
                            break;
                        case BeatTypes.Characters:
                            dto = DoCharactersBeat(dto);
                            break;
                        default:
                            dto.AddHistory("unknown beat type");
                            break;
                    }
                    dto.Beat.FinishTime = DateTime.Now.ToUniversalTime();
                    Factory.Job.AddAndSave(dto.Beat);
                    Factory.Job.AddRangeAndSave(dto.History);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    throw;
                }
                finally
                {
                    if (type == BeatTypes.Characters)
                    {
                        if (!Factory.Job.UnblockBilling())
                        {
                            throw new Exception("Биллинг был разблокирован раньше времени");
                        }
                    }
                }
            });
            if (wait)
            {
                task.Wait();
            }
            return $"Пересчет для {cycle.Token}_{cycle.Number} запущен ";
        }

        private JobLifeDto DoCharactersBeat(JobLifeDto beat)
        {
            Console.WriteLine("Запущен пересчет персонажей");
            var sins = Factory.Billing.GetActiveSins(s => s.Wallet, s => s.Character);
            Console.WriteLine($"Обрабатывается {sins.Count} персонажей");
            var charactersLoaded = false;
            var incomeList = new ConcurrentQueue<ImportDto>();
            var processedList = new ConcurrentQueue<ImportDto>();
            var errorList = new ConcurrentQueue<ImportDto>();
            var lsDto = new JobLifeStyleDto();
            var taskLoad = Task.Run(() =>
            {
                Console.WriteLine("Пошла внешняя загрузка персонажей");
                LoadCharacters(sins, incomeList);
                Console.WriteLine("Внешняя загрузка персонажей закончена");
                charactersLoaded = true;
            });
            var taskProcess = Task.Run(() =>
            {
                var k = (Factory.Settings.GetDecimalValue(SystemSettingsEnum.karma_k) / 100);
                var x1 = ((lsDto.SumRents/lsDto.Count) * k) / (lsDto.SumKarma/lsDto.Count);
                if (x1 > lsDto.KarmaK)
                {
                    lsDto.KarmaK = x1;
                }

                while (!charactersLoaded || !incomeList.IsEmpty)
                {
                    if (!charactersLoaded && incomeList.Count < _bulk)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    var parallelList = new List<ImportDto>();
                    ImportDto loaded;
                    while (incomeList.TryDequeue(out loaded))
                    {
                        if (string.IsNullOrEmpty(loaded.ErrorText))
                        {
                            parallelList.Add(loaded);
                        }
                        else
                        {
                            errorList.Enqueue(loaded);
                        }
                    }
                    Console.WriteLine($"Внутренняя обработка пачки из {parallelList.Count} персонажей");
                    ParallelCharacters(parallelList, lsDto, processedList, errorList);
                    Console.WriteLine($"Внутренняя обработка пачки из {parallelList.Count} персонажей");
                }
            });
            Task.WaitAll(taskLoad, taskProcess);
            Console.WriteLine("Пересчеты персонажей закончены, записывается история и ошибки");
            foreach (var error in errorList)
            {
                beat.AddHistory($"ошибка обработки {error.Sin.Character.Model}: {error.ErrorText}");
            }
            try
            {
                var values = ProcessLifestyle(lsDto);
                var message = $"Значения для lifestyle {values}";
                beat.AddHistory(message);
            }
            catch (Exception e)
            {
                beat.AddHistory(e.ToString());
            }
            return beat;
        }

        private void ParallelCharacters(List<ImportDto> characters, JobLifeStyleDto lsDto, ConcurrentQueue<ImportDto> processed, ConcurrentQueue<ImportDto> errors)
        {
            Parallel.ForEach(characters, new ParallelOptions { MaxDegreeOfParallelism = _bulk },
                () => new BeatCharacterLocalDto(),
                (character, loopState, localDto) =>
                {
                    try
                    {
                        localDto = ProcessModelCharacter(character, lsDto.KarmaK);
                        processed.Enqueue(character);
                    }
                    catch (Exception e)
                    {
                        character.ErrorText = e.ToString();
                        errors.Enqueue(character);
                    }
                    lsDto.AddConcurrent(localDto);
                    return localDto;
                },
                (final) =>
                {
                    
                });
        }

        private string ProcessLifestyle(JobLifeStyleDto dto)
        {
            var db = new LifeStyleAppDto();
            db.Bronze = dto.Bronze();
            db.Silver = dto.Silver();
            db.Gold = dto.Gold();
            db.Platinum = dto.Platinum();
            db.ForecastBronze = dto.ForecastBronze();
            db.ForecastSilver = dto.ForecastSilver();
            db.ForecastGold = dto.ForecastGold();
            db.ForecastPlatinum = dto.ForecastPlatinum();
            var settings = IocContainer.Get<ISettingsManager>();
            var value = Serialization.Serializer.ToJSON(db);
            var beat = Serialization.Serializer.ToJSON(dto);
            settings.SetValue(SystemSettingsEnum.ls_dto, value);
            settings.SetValue(SystemSettingsEnum.beat_characters_dto, beat);
            return beat;
        }

        private BeatCharacterLocalDto ProcessModelCharacter(ImportDto character, decimal karmaK)
        {
            var billing = IocContainer.Get<IBillingManager>();
            var workModel = new WorkModelDto
            {
                Dividends1 = character?.EreminModel?.workModel?.passiveAbilities?.Any(p => p.id == "dividends-1") ?? false,
                Dividends2 = character?.EreminModel?.workModel?.passiveAbilities?.Any(p => p.id == "dividends-2") ?? false,
                Dividends3 = character?.EreminModel?.workModel?.passiveAbilities?.Any(p => p.id == "dividends-3") ?? false,
                StockGainPercentage = character?.EreminModel?.workModel?.billing?.stockGainPercentage ?? 0,
                KarmaCount = character?.EreminModel?.workModel?.karma?.spent ?? 0
            };
            var dto = billing.ProcessCharacterBeat(character.Sin.Id, workModel, karmaK);
            try
            {
                EreminPushAdapter.SendNotification(character.Sin.Character.Model, "Кошелек", "Экономический пересчет завершен");
            }
            catch (Exception e)
            {
                LogException(e);
            }
            return dto;
        }

        private void LoadCharacters(List<SIN> sins, ConcurrentQueue<ImportDto> concurrent)
        {
            var erService = new EreminService();
            Parallel.ForEach(sins, new ParallelOptions { MaxDegreeOfParallelism = 5 }, sin =>
                {
                    var dto = new ImportDto { Sin = sin };
                    try
                    {
                        dto.EreminModel = erService.GetCharacter(sin.Character.Model);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                        dto.ErrorText = e.Message;
                    }
                    concurrent.Enqueue(dto);
                });
        }

        private JobLifeDto DoItemsBeat(JobLifeDto beat)
        {
            Console.WriteLine("Запущен пересчет товаров");
            //Получить список корпораций с специализациями
            var corporations = Factory.Billing.GetList<CorporationWallet>(c => true, c => c.Specialisations);
            var inflation = Factory.Settings.GetDecimalValue(SystemSettingsEnum.pre_inflation);
            foreach (var corporation in corporations)
            {
                corporation.LastSkuSold = corporation.SkuSold;
                corporation.SkuSold = 0;
                corporation.LastKPI = corporation.CurrentKPI;
                corporation.CurrentKPI = 0;
                var skus = Factory.Billing.GetList<Sku>(s => s.CorporationId == corporation.Id, s => s.Nomenklatura.Specialisation);
                var specs = corporation.Specialisations.Select(s => s.SpecialisationId);
                foreach (var sku in skus)
                {
                    sku.Count = sku.SkuBaseCount ?? sku.Nomenklatura.BaseCount;
                    decimal price = 0;
                    if(sku.SkuBasePrice == null)
                    {
                        price = BillingHelper.GetSpecialisationPrice(corporation.Specialisations.FirstOrDefault(s => s.SpecialisationId == sku.Nomenklatura.SpecialisationId), sku.Nomenklatura);
                    }
                    else
                    {
                        price = sku.SkuBasePrice ?? 0;
                    }

                    sku.Price = price * inflation;
                }
                Factory.Billing.SaveContext();
                Console.WriteLine($"Корпорация {corporation.Name} обработана");
            }
            return beat;
        }

        private void LogException(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }

    }
}
