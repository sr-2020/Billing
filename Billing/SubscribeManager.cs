using Core;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface ISubscribeManager : IBaseRepository
    {
        void AbilityLog(string key, string message);
    }


    public class SubscribeManager : BaseEntityRepository, ISubscribeManager
    {
        public void AbilityLog(string key, string message)
        {
            var log = new BillingAbilityLog
            {
                Key = key,
                Message = message
            };
            Context.Add(log);
            SaveContext();
        }
    }
}
