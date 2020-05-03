using Billing;
using Core.Model;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class RentaJob : BaseJob
    {
        public override string JobName { get => "RentaJob"; }
        private ISettingsManager _settingManager = IocContainer.Get<ISettingsManager>();
        private IBillingManager _billingManager = IocContainer.Get<IBillingManager>();

        public override void Handle()
        {
            base.Handle();
            //lock
            Start();
            try
            {
                _billingManager.ProcessRentas();
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
            Console.WriteLine("RentaJob processing start");
            var processing = _settingManager.GetBoolValue("block");
            if (processing)
                return;
            _settingManager.SetValue("block", "true");
        }

        private void Finish()
        {
            Console.WriteLine("RentaJob processing finish");
            _settingManager.SetValue("block", "false");
            var version = _settingManager.GetIntValue(Core.Primitives.SystemSettingsEnum.eversion);
            version++;
            _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.eversion, version.ToString());
        }

    }
}
