using Billing;
using Core;
using Jobs;
using Settings;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Init
{
    public class BaseRegistry : Registry
    {
        public BaseRegistry()
        {
            RegisterContexts();
        }
        protected virtual void RegisterContexts()
        {
            For<ISettingsManager>().Use(new SettingsManager()).SetLifecycleTo(Lifecycles.Transient);
            For<IJobManager>().Use(new JobManager()).SetLifecycleTo(Lifecycles.Transient);
            For<IBaseRepository>().Use(new BaseEntityRepository()).SetLifecycleTo(Lifecycles.Transient);
            For<IBillingManager>().Use(new BillingManager()).SetLifecycleTo(Lifecycles.Transient);
        }
    }
}
