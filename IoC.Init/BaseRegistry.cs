﻿using Billing;
using Core;
using Jobs;
using Scoringspace;
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
            For<ISettingsManager>().Use<SettingsManager>().SetLifecycleTo(Lifecycles.Transient);
            For<IJobManager>().Use<JobManager>().SetLifecycleTo(Lifecycles.Transient);
            For<IBillingManager>().Use<BillingManager>().SetLifecycleTo(Lifecycles.Transient);
            For<IShopManager>().Use<ShopManager>().SetLifecycleTo(Lifecycles.Transient);
            For<IInsuranceManager>().Use<InsuranceManager>().SetLifecycleTo(Lifecycles.Transient);
            For<IScoringManager>().Use<ScoringManager>().SetLifecycleTo(Lifecycles.Transient);
            For<ISubscribeManager>().Use<SubscribeManager>().SetLifecycleTo(Lifecycles.Singleton);
            For<IAdminManager>().Use<AdminManager>().SetLifecycleTo(Lifecycles.Singleton);
        }
    }
}
