using Core;
using Core.Model;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jobs
{
    public abstract class BaseJob
    {
        public JobConfig Config { get; set; }

        public BaseJob() { }

        public virtual void DoJob()
        {
            try
            {
                //TODO add logging
                Console.WriteLine(SystemHelper.ConvertDateTimeToMoscow());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public virtual List<DateTime> GetJobSchedules(bool localTimeZone = true)
        {
            var result = new List<DateTime>();
            var start = Config.StartTime;
            var now = SystemHelper.ConvertDateTimeToMoscow();
            if (now > start)
                start = now;
            for (var date = start; date <= Config.EndTime; date = date.AddMinutes(Config.IntervalInMinutes))
                result.Add(localTimeZone ? SystemHelper.ConvertDateTimeToLocal(date) : date);
            return result;
        }
    }
}
