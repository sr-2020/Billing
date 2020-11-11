using Billing;
using Core.Model;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class PeriodJob : BaseJob
    {
        public override string JobName { get => "PeriodJob"; }
        private ISettingsManager _settingManager = IocContainer.Get<ISettingsManager>();
        private IBillingManager _billingManager = IocContainer.Get<IBillingManager>();

        public override void Handle()
        {
            base.Handle();
            var processing = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.block);
            if (processing)
                return;
            //lock
            Start();
            try
            {
                _billingManager.ProcessPeriod();
            }
            catch (Exception e)
            {
                Console.WriteLine("ошибка обработки рент");
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                //unlock
                Finish();
            }
        }

        private void Start()
        {
            Console.WriteLine("PeriodJob processing start");
            _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "true");
        }

        private void Finish()
        {
            Console.WriteLine("PeriodJob processing finish");
            _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "false");
            var version = _settingManager.GetIntValue(Core.Primitives.SystemSettingsEnum.eversion);
            version++;
            _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.eversion, version.ToString());
        }

    }
}
