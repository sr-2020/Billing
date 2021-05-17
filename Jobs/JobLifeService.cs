using Billing;
using Billing.Dto;
using Billing.Dto.Shop;
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
    public class JobLifeService
    {
        public JobLifeService()
        {
            Factory = new ManagerFactory();
        }
        ManagerFactory Factory { get; set; }

        public const string BillingException = "Биллинг заблокирован";

        public string ToggleCycle(string token = "")
        {
            if(string.IsNullOrEmpty(token))
            {
                token = GetCurrentToken();
            }
            var cycle = Factory.Job.GetLastCycle(token);
            if (cycle == null)
            {
                cycle = new BillingCycle()
                {
                    Number = 1
                };
            }
            else if (cycle.IsActive == false)
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

        public string DoBeat(BeatTypes type = BeatTypes.Test, string token = "")
        {
            if(string.IsNullOrEmpty(token))
            {
                token = GetCurrentToken();
            }
            var cycle = Factory.Job.GetLastCycle(token);
            if (cycle == null || !cycle.IsActive)
            {
                return "цикл не запущен";
            }
            if (!Factory.Job.BlockBilling())
            {
                throw new BillingException(BillingException);
            }
            Task.Run(() =>
            {
                try
                {
                    var beat = Factory.Job.GetLastBeat(cycle.Id, type);
                    var newBeat = new BillingBeat();
                    newBeat.Number = beat != null ? beat.Number++ : 1;
                    newBeat.StartTime = DateTime.Now;
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
                    dto.Beat.FinishTime = DateTime.Now;
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
                    if (!Factory.Job.UnblockBilling())
                    {
                        throw new Exception("Биллинг был разблокирован раньше времени");
                    }
                }

            });
            return $"Пересчет для {cycle.Token}_{cycle.Number} запущен ";
        }

        private JobLifeDto DoCharactersBeat(JobLifeDto beat)
        {
            Console.WriteLine("Запущен пересчет");
            var sins = Factory.Billing.GetActiveSins(s => s.Wallet, s => s.Character);
            Console.WriteLine($"Обрабатывается {sins.Count} персонажей");
            var charactersLoaded = false;
            var concurrent = new ConcurrentQueue<CharacterDto>();
            var processedList = new List<CharacterDto>();
            var errorList = new List<CharacterDto>();
            var lsDto = new JobLifeStyleDto();
            var taskLoad = Task.Run(() =>
            {
                LoadCharacters(sins, concurrent);
                charactersLoaded = true;
            });
            var taskProcess = Task.Run(() =>
            {
                while (!charactersLoaded || !concurrent.IsEmpty)
                {
                    CharacterDto loaded;
                    if (!concurrent.TryDequeue(out loaded))
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (string.IsNullOrEmpty(loaded.ErrorText))
                    {
                        try
                        {
                            lsDto = ProcessModelCharacter(loaded, lsDto);
                            lsDto.Count++;
                            processedList.Add(loaded);
                        }
                        catch (Exception e)
                        {
                            loaded.ErrorText = e.ToString();
                            errorList.Add(loaded);
                        }
                    }
                    else
                    {
                        errorList.Add(loaded);
                    }
                }
            });
            Task.WaitAll(taskLoad, taskProcess);
            Console.WriteLine("Пересчеты персонажей закончены, записывается история и ошибки");
            foreach (var error in errorList)
            {
                beat.AddHistory($"ошибка обработки {error.Sin.CharacterId}: {error.ErrorText}");
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
            var save = Serialization.Serializer.ToJSON(dto);
            settings.SetValue(SystemSettingsEnum.ls_dto, value);
            return save;
        }

        private JobLifeStyleDto ProcessModelCharacter(CharacterDto character, JobLifeStyleDto dto)
        {
            var billing = IocContainer.Get<IBillingManager>();
            var d1 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-1");
            var d2 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-2");
            var d3 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-3");
            dto = billing.ProcessCharacterBeat(character.Sin.Id, character.EreminModel.workModel.karma.spent ?? 0, d1 ?? false, d2 ?? false, d3 ?? false, dto);
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

        private void LoadCharacters(List<SIN> sins, ConcurrentQueue<CharacterDto> concurrent)
        {
            var erService = new EreminService();
            Parallel.ForEach(sins, new ParallelOptions { MaxDegreeOfParallelism = 5 }, sin =>
                {

                    var dto = new CharacterDto { Sin = sin };
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

            return beat;
        }

        private string GetCurrentToken()
        {
            return Factory.Settings.GetValue(Core.Primitives.SystemSettingsEnum.token);
        }

        private void DoIkar(List<SIN> sins)
        {
            //var k = _settingManager.GetDecimalValue(Core.Primitives.SystemSettingsEnum.ikar_k);
            //var count = Billing.ProcessIkar(sins, k);
            //Console.WriteLine($"Пересчет ИКАР завершен, начислено для {count} персонажей с коэффициентом {k}");
            //CurrentBeat.SuccessIkar = true;
        }

        private void DoScoring(List<SIN> sins)
        {
            //var metatype_count = 0;
            ////TODO пересчет по скорингу
            //foreach (var sin in sins)
            //{
            //    if (sin.OldMetaTypeId != sin.MetatypeId)
            //    {
            //        Scoring.OnMetatypeChanged(sin);
            //        sin.OldMetaTypeId = sin.MetatypeId;
            //        metatype_count++;
            //    }
            //}
            //Billing.SaveContext();
            //Console.WriteLine($"Пересчет скоринга успешен, метатип пересчитан для {metatype_count} синов");
            //CurrentBeat.SuccessScoring = true;
        }

        private string DoRentas()
        {
            //var rentasCount = Billing.ProcessRentas(sins);
            //Console.WriteLine($"Пересчет рент завершен, обработано {rentasCount} рент");
            //CurrentBeat.SuccessRent = true;
            return $"";
        }

        private void LogException(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }

    }
}
