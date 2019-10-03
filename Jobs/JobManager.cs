using Core;
using Core.Model;
using IoC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jobs
{
    public interface IJobManager
    {
        BaseJob GetJob(string id);
        JobConfig GetConfig(string id);
    }

    public class JobManager : IJobManager
    {
        public BaseJob GetJob(string name)
        {
            switch (name)
            {
                case "test":
                    return new TestJob();
                default:
                    break;
            }
            throw new NotImplementedException("Job not configured");
        }

        public JobConfig GetConfig(string id)
        {
            var config = new JobConfig();
            config = new JobConfig();
            config.Name = GetJobNameById(id);
            config.Instance = GetInstance(id);

            var settingsManager = IocContainer.Get<IBaseRepository>();
            var settings = settingsManager.GetList<SystemSettings>(s => s.Key.StartsWith($"job_{id}_"));
            config.CronExpression = GetCron(settings);
            config.StartTime = GetTime(settings, "start");
            config.EndTime = GetTime(settings, "end"); ;
            return config;
        }

        private DateTime GetTime(List<SystemSettings> settings, string key)
        {
            var provider = CultureInfo.InvariantCulture;
            var _dateTimeFormat = "dd.MM.yyyy HH:mm:ss";
            var setting = settings.FirstOrDefault(s => s.Key.EndsWith(key));
            return DateTime.ParseExact(setting.Value, _dateTimeFormat, provider);
        }

        private string GetCron(List<SystemSettings> settings)
        {
            var setting = settings.FirstOrDefault(s => s.Key.EndsWith("cron"));
            return setting.Value;
        }

        private string GetInstance(string id)
        {
            var index = id.IndexOf(":") + 1;
            return index > 0 ? id.Substring(index, id.Length - index) : "default";
        }

        private string GetJobNameById(string id)
        {
            var index = id.IndexOf(":");
            return id.Substring(0, index > 0 ? index : id.Length);
        }
    }
}
