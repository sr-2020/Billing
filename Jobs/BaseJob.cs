using Core;
using Core.Model;
using Hangfire;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jobs
{
    public abstract class BaseJob
    {
        public BaseJob() { }

        public virtual void ScheduleJob(string id, DateTime startTime, DateTime endTime, int interval)
        {
            var startId = BackgroundJob.Schedule(() => StartJob(id, interval), new DateTimeOffset(SystemHelper.ConvertDateTimeToLocal(startTime)));
            var endId = BackgroundJob.Schedule(() => StopJob(id), new DateTimeOffset(SystemHelper.ConvertDateTimeToLocal(endTime)));
        }

        protected virtual void StartJob(string id, int interval)
        {
            RecurringJob.AddOrUpdate(id, () => DoJob(), $"*/{interval} * * * *");
        }

        protected virtual void StopJob(string id)
        {
            RecurringJob.RemoveIfExists(id);
        }

        protected virtual void DoJob()
        {
            try
            {
                //TODO add logging
                Console.WriteLine(SystemHelper.ConvertDateTimeToMoscow());
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
