using Billing;
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
    public class JobLife
    {

        private ISettingsManager _settingManager = IocContainer.Get<ISettingsManager>();
        private Lazy<IJobManager> _lazyJob { get; set; } = new Lazy<IJobManager>(IocContainer.Get<IJobManager>);
        private Lazy<IBillingManager> _lazyBilling { get; set; } = new Lazy<IBillingManager>(IocContainer.Get<IBillingManager>);
        private Lazy<IShopManager> _lazyShop { get; set; } = new Lazy<IShopManager>(IocContainer.Get<IShopManager>);
        private Lazy<IScoringManager> _lazyScoring { get; set; } = new Lazy<IScoringManager>(IocContainer.Get<IScoringManager>);
        private BillingBeat CurrentBeat { get; set; }
        private IJobManager Job => _lazyJob.Value;
        private IBillingManager Billing => _lazyBilling.Value;
        private IShopManager Shop => _lazyShop.Value;
        private IScoringManager Scoring => _lazyScoring.Value;

        private bool _blocked;
        private bool _broken;
        private bool _beatCycle;
        private bool _beatPeriod;

        public JobLife()
        {
            Init();
        }

        private void Init()
        {
            //try
            //{
            //    var lastBeat = Job.GetLastBeat();
            //    if (lastBeat == null)
            //    {
            //        lastBeat = new BillingBeat
            //        {
            //            Period = 0
            //        };
            //    }
            //    var newBeat = new BillingBeat
            //    {
            //        StartTime = DateTime.Now,
            //        Period = lastBeat.Period
            //    };
            //    var processing = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.block);
            //    if (processing)
            //    {
            //        Console.Error.WriteLine("Процесс пересчета уже запущен! Попытка повторного запуска");
            //        _blocked = true;
            //        return;
            //    }

            //    Job.AddAndSave(newBeat);
            //    CurrentBeat = newBeat;
            //    //block
            //    _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "true");
            //    Console.WriteLine("Биллинг заблокирован");
            //}
            //catch (Exception e)
            //{
            //    LogException(e);
            //    _broken = true;
            //}
        }

        public void Start()
        {
            //_beatCycle = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.beat_cycle);
            //_beatPeriod = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.beat_period);
            //Task.Run(() =>
            //{
            //    if (_beatCycle)
            //    {
            //        try
            //        {
            //            DoCycle();
            //        }
            //        catch (Exception e)
            //        {
            //            LogException(e);
            //        }
            //        finally
            //        {
            //            _beatCycle = false;
            //        }
            //    }
            //    if (_beatPeriod)
            //    {
            //        try
            //        {
            //            DoPeriod();
            //        }
            //        catch (Exception e)
            //        {
            //            LogException(e);
            //        }
            //        finally
            //        {
            //            _beatPeriod = false;
            //        }
            //    }
            //    Finish();
            //});
        }

        private void DoCycle()
        {
            //if (_broken || _blocked)
            //{
            //    return;
            //}
            //var periodCount = _settingManager.GetIntValue(Core.Primitives.SystemSettingsEnum.period_count);
            //if (periodCount == 0)
            //{
            //    Console.WriteLine("period_count = 0, цикл не пересчитывается");
            //    return;
            //}
            //var localPeriod = CurrentBeat.Period % periodCount;
            //int localCycle = CurrentBeat.Period / periodCount;
            //Console.WriteLine($"{CurrentBeat.Period} пересчет, {localCycle} цикл");
            //if (localPeriod != 0)
            //{
            //    Console.WriteLine($"Цикл пересчитывается на каждом {periodCount} пересчете, в данном пересчете цикл не пересчитывается");
            //    return;
            //}
            //try
            //{
            //    Console.WriteLine("Пересчет инфляции");
            //    var k = _settingManager.GetDecimalValue(Core.Primitives.SystemSettingsEnum.pre_inflation);
            //    var count = Shop.ProcessInflation(k);
            //    Console.WriteLine($"Умножение цены для {count} товаров на коэффициент {k}");
            //    CurrentBeat.SuccessInflation = true;
            //}
            //catch (Exception e)
            //{
            //    LogException(e);
            //}
        }

        private void DoPeriod()
        {
            //if (_broken || _blocked)
            //    return;
            //CurrentBeat.Period++;
            //Console.WriteLine($"Запуск {CurrentBeat.Period} периода");
            //var sins = Billing.GetActiveSins();
            //Console.WriteLine($"Получено {sins.Count} персонажей");
            //try
            //{
            //    DoScoring(sins);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Ошибка DoScoring");
            //    LogException(e);
            //}
            //try
            //{
            //    DoRentas(sins);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Ошибка DoRentas");
            //    LogException(e);
            //}
            //try
            //{
            //    DoKarma(sins);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Ошибка DoKarma");
            //    LogException(e);
            //}
            //try
            //{
            //    DoIkar(sins);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Ошибка DoIkar");
            //    LogException(e);
            //}
            //try
            //{
            //    DoPush(sins);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Ошибка DoPush");
            //    LogException(e);
            //}

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

        private void DoKarma(List<SIN> sins)
        {
            //var k = _settingManager.GetDecimalValue(Core.Primitives.SystemSettingsEnum.karma_k);
            //Console.WriteLine("Пересчет кармы начался");
            //var count = Billing.ProcessKarma(sins, k);
            //Console.WriteLine($"Пересчет кармы завершен, начислено для {count} персонажей с коэффициентом {k}");
            //CurrentBeat.SuccessWork = true;
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

        private void DoRentas(List<SIN> sins)
        {
            //var rentasCount = Billing.ProcessRentas(sins);
            //Console.WriteLine($"Пересчет рент завершен, обработано {rentasCount} рент");
            //CurrentBeat.SuccessRent = true;
        }

        private void Finish()
        {
            //if (_blocked)
            //    return;
            //_settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "false");
            //Console.WriteLine("Биллинг разблокирован");
            //CurrentBeat.FinishTime = DateTime.Now;
            //Job.SaveContext();
            //Console.WriteLine("Пересчет закончен");
            //return;
        }

        private void LogException(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }

    }
}
