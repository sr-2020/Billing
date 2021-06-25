using IoC;
using Scoringspace;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class ManagerFactory
    {
        public ISettingsManager Settings { get; } = IocContainer.Get<ISettingsManager>();
        private Lazy<IBillingManager> _lazyBilling { get; set; } = new Lazy<IBillingManager>(IocContainer.Get<IBillingManager>);
        public IBillingManager Billing => _lazyBilling.Value;
        private Lazy<IJobManager> _lazyJob { get; set; } = new Lazy<IJobManager>(IocContainer.Get<IJobManager>);
        public IJobManager Job => _lazyJob.Value;
        private Lazy<IScoringManager> _lazyScoring { get; set; } = new Lazy<IScoringManager>(IocContainer.Get<ScoringManager>);
        public IScoringManager Scoring => _lazyScoring.Value;
        private Lazy<IShopManager> _lazyShop { get; set; } = new Lazy<IShopManager>(IocContainer.Get<IShopManager>);
        public IShopManager Shop => _lazyShop.Value;
    }
}
