using Billing;
using Core;
using Core.Model;
using InternalServices;
using IoC;
using Scoringspace;
using Settings;
using System;
using System.Collections.Generic;
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

        private Lazy<IJobManager> _lazyJob { get; set; } = new Lazy<IJobManager>(IocContainer.Get<IJobManager>);
        private Lazy<IBillingManager> _lazyBilling { get; set; } = new Lazy<IBillingManager>(IocContainer.Get<IBillingManager>);
        private Lazy<IShopManager> _lazyShop { get; set; } = new Lazy<IShopManager>(IocContainer.Get<IShopManager>);
        private Lazy<IScoringManager> _lazyScoring { get; set; } = new Lazy<IScoringManager>(IocContainer.Get<IScoringManager>);
        private IJobManager Job => _lazyJob.Value;
        private IBillingManager Billing => _lazyBilling.Value;
        private IShopManager Shop => _lazyShop.Value;
        private IScoringManager Scoring => _lazyScoring.Value;


        public BillingCycle ToggleCycle(string token = "")
        {
            token = CheckToken(token);
            var cycle = Job.GetLastCycle(token);
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
            Job.AddAndSave(cycle);
            return cycle;
        }

        public BillingBeat DoBeat(string token = "")
        {
            BillingHelper.BillingBlocked();
            token = CheckToken(token);
            var cycle = Job.GetLastCycle(token);
            if (cycle == null)
                return null;
            if (!cycle.IsActive)
                return null;
            if (!Job.BlockBilling())
            {
                throw new Exception("Попытка повторной блокировки биллинга");
            }
            var beat = Job.GetLastBeat(cycle.Id);
            var newBeat = new BillingBeat();
            newBeat.Number = beat != null ? beat.Number++ : 1;
            newBeat.StartTime = DateTime.Now;
            newBeat.CycleId = cycle.Id;
            Job.AddAndSave(newBeat);
            var actions = Job.GetAllActions();
            foreach (var action in actions)
            {
                if (!CheckActionTime(action, cycle.Number, beat.Number))
                {
                    continue;
                }
                var success = false;
                var comment = string.Empty;
                try
                {
                    comment = DoAction(action);
                    success = true;
                }
                catch (Exception e)
                {
                    comment = e.ToString();
                    LogException(e);
                }
                finally
                {
                    var history = new BeatHistory
                    {
                        ActionId = action.Id,
                        Comment = comment,
                        Success = success,
                        BeatId = newBeat.Id
                    };
                    Job.AddAndSave(history);
                }
            }
            newBeat.FinishTime = DateTime.Now;
            Job.AddAndSave(newBeat);
            if (!Job.UnblockBilling())
            {
                throw new Exception("Биллинг был разблокирован раньше времени");
            }
            return newBeat;
        }

        private bool CheckActionTime(BillingAction action, int cycle, int beat)
        {
            if (action.Cycle != 0)
            {
                if (cycle % action.Cycle != 0)
                    return false;
            }
            if (action.Beat != 0)
            {
                if (beat % action.Beat != 0)
                    return false;
            }
            return true;
        }

        private string DoAction(BillingAction action)
        {
            switch (action.Alias)
            {
                case "renta":
                    return DoRentas();
                case "karma":

                    break;

                case "ikar":

                    break;

                case "scoring":

                    break;
                case "inflation":

                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        private string CheckToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = Factory.Settings.GetValue(Core.Primitives.SystemSettingsEnum.token);
            }
            return token;
        }

        private void DoPush(List<SIN> sins)
        {
            foreach (var sin in sins)
            {
                try
                {
                    EreminPushAdapter.SendNotification(sin.Character.Model, "Кошелек", "Пересчет экономического периода завершен");
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                }

            }
        }

        private string DoKarma(List<SIN> sins)
        {
            //var k = _settingManager.GetDecimalValue(Core.Primitives.SystemSettingsEnum.karma_k);
            //Console.WriteLine("Пересчет кармы начался");
            //var count = Billing.ProcessKarma(sins, k);
            //Console.WriteLine($"Пересчет кармы завершен, начислено для {count} персонажей с коэффициентом {k}");
            //CurrentBeat.SuccessWork = true;
            return $"";
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
