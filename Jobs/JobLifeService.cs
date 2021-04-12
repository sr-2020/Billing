using Billing;
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

        public string ToggleCycle(string token = "")
        {
            token = CheckToken(token);
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
            return $"{cycle.Token}_{cycle.Number} {(cycle.IsActive ? "стартовал": "остановлен")}";
        }

        public string DoBeat(string token = "", BeatTypes type = BeatTypes.Test)
        {
            BillingHelper.BillingBlocked();
            token = CheckToken(token);
            var cycle = Factory.Job.GetLastCycle(token);
            if (cycle == null || !cycle.IsActive)
            {
                return "цикл не запущен";
            }
            if (!Factory.Job.BlockBilling())
            {
                throw new Exception("Попытка повторной блокировки биллинга");
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
                    Factory.Job.AddRange(dto.History);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
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
            var sins = Factory.Billing.GetActiveSins(s => s.Wallet, s => s.Character);
            var charactersLoaded = false;
            var concurrent = new ConcurrentQueue<CharacterDto>();
            var taskLoad = Task.Run(() =>
            {
                LoadCharacters(sins, concurrent);
                charactersLoaded = true;
            });
            var processedList = new List<CharacterDto>();
            var errorList = new List<CharacterDto>();
            var taskAll = Task.Run(() =>
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
                            processedList.Add(ProcessModelCharacter(loaded));
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
            Task.WaitAll(taskLoad, taskAll);
            return beat;
        }

        private CharacterDto ProcessModelCharacter(CharacterDto character)
        {
            var billing = IocContainer.Get<IBillingManager>();
            var d1 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-1");
            var d2 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-2");
            var d3 = character.EreminModel.workModel.passiveAbilities?.Any(p => p.id == "dividends-3");
            billing.ProcessCharacterBeat(character.Sin.Id, character.EreminModel.workModel.karma.spent ?? 0, d1 ?? false, d2 ?? false, d3 ?? false);
            try
            {
                EreminPushAdapter.SendNotification(character.Sin.Character.Model, "Кошелек", "Экономический пересчет завершен");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return character;
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
                        dto.ErrorText = e.Message;
                    }
                    concurrent.Enqueue(dto);
                });
        }

        private JobLifeDto DoItemsBeat(JobLifeDto beat)
        {

            return beat;
        }

        private string CheckToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = Factory.Settings.GetValue(Core.Primitives.SystemSettingsEnum.token);
            }
            return token;
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
