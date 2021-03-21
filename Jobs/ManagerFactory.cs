using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class ManagerFactory
    {
        public ISettingsManager Settings { get; } = IocContainer.Get<ISettingsManager>();


    }
}
