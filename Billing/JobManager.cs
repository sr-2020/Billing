using Core;
using Core.Model;
using Core.Primitives;
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

namespace Billing
{
    public interface IJobManager : IBaseRepository
    {
        BillingCycle GetLastCycle(string token);
        BillingBeat GetLastBeatAsNoTracking(int cycleId, BeatTypes type);
        BillingCycle GetLastCycle();
        BillingBeat GetLastBeatAsNoTracking(BeatTypes type);
        bool BlockBilling();
        bool UnblockBilling();
    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        ISettingsManager _settings = IocContainer.Get<ISettingsManager>();

        public BillingCycle GetLastCycle(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = GetCurrentToken();
            }
            var cycle = Query<BillingCycle>()
                .Where(c => c.Token == token)
                .OrderByDescending(c => c.Number)
                .FirstOrDefault();
            return cycle;
        }

        public BillingBeat GetLastBeatAsNoTracking(int cycleId, BeatTypes type)
        {
            var typeint = (int)type;
            var beat = QueryAsNoTracking<BillingBeat>()
                .Where(b => b.CycleId == cycleId && b.BeatType == typeint)
                .OrderByDescending(c => c.Number)
                .FirstOrDefault();
            return beat;
        }

        public BillingCycle GetLastCycle()
        {
            var token = GetCurrentToken();
            return GetLastCycle(token);
        }

        public BillingBeat GetLastBeatAsNoTracking(BeatTypes type)
        {
            var cycle = GetLastCycle();
            return GetLastBeatAsNoTracking(cycle.Id, type);
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
        private string GetCurrentToken()
        {
            return _settings.GetValue(Core.Primitives.SystemSettingsEnum.token);
        }
    }
}
