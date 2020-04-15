using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class TestJob : BaseJob
    {
        public override string JobName => "Test";
        public override void Handle()
        {
            base.Handle();
            var manager = IocContainer.Get<ISettingsManager>();
            var oldValue = manager.GetIntValue("test");
            manager.SetValue("test", (++oldValue).ToString());
        }

    }
}
