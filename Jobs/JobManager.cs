using Billing;
using Core;
using Core.Model;
using Core.Primitives;
using Hangfire;
using IoC;
using NCrontab;
using Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public interface IJobManager : IBaseRepository
    {
        BillingCycle GetLastCycle(string token);
        BillingBeat GetLastBeat(int cycleId);
        bool BlockBilling();
        bool UnblockBilling();
        List<BillingAction> GetAllActions();

    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        ISettingsManager _settings = IocContainer.Get<ISettingsManager>();

        public BillingCycle GetLastCycle(string token)
        {
            var cycle = Query<BillingCycle>()
                .Where(c => c.Token == token)
                .OrderByDescending(c => c.Number)
                .FirstOrDefault();
            return cycle;
        }
        public BillingBeat GetLastBeat(int cycleId)
        {
            var beat = Query<BillingBeat>()
                .Where(b => b.CycleId == cycleId)
                .OrderByDescending(c => c.Number)
                .FirstOrDefault();
            return beat;
        }

        public bool BlockBilling()
        {
            var blocked = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (blocked)
                return false;
            _settings.SetValue(SystemSettingsEnum.block, "true");
            return true;
        }

        public bool UnblockBilling()
        {
            var blocked = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (!blocked)
                return false;
            _settings.SetValue(SystemSettingsEnum.block, "false");
            return true;
        }

        public List<BillingAction> GetAllActions()
        {
            return GetList<BillingAction>(a => a.Enabled).OrderBy(a => a.Order).ToList();
        }
    }
}
