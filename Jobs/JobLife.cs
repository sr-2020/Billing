using Billing;
using Core.Model;
using IoC;
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
        private BillingBeat CurrentBeat { get; set; }
        private IJobManager Job => _lazyJob.Value;
        private IBillingManager Billing => _lazyBilling.Value;
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
            try
            {
                var lastBeat = Job.GetLastBeat();
                if (lastBeat == null)
                {
                    lastBeat = new BillingBeat
                    {
                        Period = 0
                    };
                }
                var newBeat = new BillingBeat
                {
                    StartTime = DateTime.Now,
                    Period = lastBeat.Period
                };
                var processing = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.block);
                if (processing)
                {
                    Console.Error.WriteLine("Процесс пересчета уже запущен! Попытка повторного запуска");
                    _blocked = true;
                    return;
                }
                newBeat.Period++;
                Console.WriteLine($"Запуск {newBeat.Period} периода");
                Job.AddAndSave(newBeat);
                CurrentBeat = newBeat;
                //lock
                _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "true");
                Console.WriteLine("Биллинг заблокирован");
            }
            catch (Exception e)
            {
                LogException(e);
                _broken = true;
            }
        }

        public void Start()
        {
            _beatCycle = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.beat_cycle);
            _beatPeriod = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.beat_period);
            if (_beatCycle)
            {
                Task.Run(() =>
                {
                    try
                    {
                        DoCycle();
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                    finally
                    {
                        _beatCycle = false;
                    }
                });
            }
            if(_beatPeriod)
            {
                Task.Run(async () =>
                {
                    while (_beatCycle)
                    {
                        await Task.Delay(3000);
                    }
                    try
                    {
                        DoPeriod();
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                    finally
                    {
                        _beatPeriod = false;
                    }
                });
            }
            Task.Run(async ()=>
            {
                while(_beatCycle || _beatPeriod)
                {
                    await Task.Delay(3000);
                }
                Finish();
            });
        }

        private void DoCycle()
        {
            if (_broken || _blocked)
            {
                return;
            }
            var periodCount = _settingManager.GetIntValue(Core.Primitives.SystemSettingsEnum.period_count);
            if(periodCount == 0)
            {
                Console.WriteLine("period_count = 0, цикл не пересчитывается");
                return;
            }    
            var localPeriod = CurrentBeat.Period % periodCount;
            int localCycle = CurrentBeat.Period / periodCount;
            Console.WriteLine($"{CurrentBeat.Period} пересчет, {localCycle} цикл, {localPeriod} период");
            if(localPeriod != 0)
            {
                Console.WriteLine($"Цикл пересчитывается на каждом {periodCount} пересчете, в данном пересчете цикл не пересчитывается");
                return;
            }
            try
            {
                Console.WriteLine("Пересчет инфляции");
                Thread.Sleep(10000);
                //TODO инфляция
                Console.WriteLine($"Умножение цены для 0 товаров на коэффициент 0");
                CurrentBeat.SuccessInflation = true;
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private void DoPeriod()
        {
            if (_broken || _blocked)
                return;
            try
            {
                //TODO пересчет по скорингу
                Console.WriteLine("Пересчет скоринга");
                Thread.Sleep(10000);
                CurrentBeat.SuccessScoring = true;
                //TODO ренты
                Console.WriteLine("Пересчет рент");
                Thread.Sleep(10000);
                CurrentBeat.SuccessRent = true;
                //TODO деньги за карму
                Console.WriteLine("Пересчет кармы");
                CurrentBeat.SuccessWork = true;
                //TODO деньги за ИКАР
                Console.WriteLine("Пересчет икар");
                CurrentBeat.SuccessIkar = true;
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private void Finish()
        {
            if (_blocked)
                return;
            _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "false");
            Console.WriteLine("Биллинг разблокирован");
            CurrentBeat.FinishTime = DateTime.Now;
            Job.SaveContext();
            Console.WriteLine("Пересчет закончен");
            return;
        }

        private void LogException(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }

    }
}
