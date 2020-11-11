using Billing;
using Core;
using Core.Model;
using Core.Primitives;
using Hangfire;
using IoC;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jobs
{
    public interface IJobManager : IBaseRepository
    {
        void ProcessCycle(string modelId);

        HangfireJob AddOrUpdateJob(int id, DateTimeOffset? start, DateTimeOffset? end, string cron, string jobname, int jobtype);
        List<HangfireJob> GetAllJobs(bool finished, int jobtype);
    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        public HangfireJob AddOrUpdateJob(int id, DateTimeOffset? start, DateTimeOffset? end, string cron, string jobname, int jobtype)
        {
            HangfireJob job = null;
            if (id > 0)
                job = Get<HangfireJob>(h => h.Id == id);
            if (job == null)
            {
                job = new HangfireJob();
            }
            if (start.HasValue && start > DateTimeOffset.Now)
            {
                job.StartTime = start.Value;
            }
            if (end.HasValue && end > DateTimeOffset.Now)
            {
                job.EndTime = end.Value;
            }
            if (SystemHelper.CronParse(cron) != null)
            {
                job.Cron = cron;
            }
            if (!string.IsNullOrEmpty(jobname))
            {
                job.JobName = jobname;
            }
            if (Enum.IsDefined(typeof(JobType), jobtype))
            {
                job.JobType = jobtype;
            }
            Add(job);
            Context.SaveChanges();
            job = CreateOrUpdateStartJob(job);

            Add(job);
            Context.SaveChanges();
            return job;
        }

        public async void ProcessCycle(string model)
        {
            var cycle = new BillingCycle
            {
                StartTime = DateTime.Now
            };
            Add(cycle);
            Context.SaveChanges();

            cycle.FinishTime = DateTime.Now;
            Context.SaveChanges();
        }

        public async void ProcessPeriod()
        {
            var job = new PeriodJob();
            await job.DoJob();
        }

        #region hangfire(obsolete)

        public List<HangfireJob> GetAllJobs(bool finished, int jobtype)
        {
            var filter = DateTimeOffset.Now;
            return GetList<HangfireJob>(j => (j.EndTime > filter || finished) && j.JobType == jobtype).ToList();
        }

        private HangfireJob CreateOrUpdateStopJob(HangfireJob job)
        {
            if (!string.IsNullOrEmpty(job.HangfireEndId))
            {
                RecurringJob.RemoveIfExists(job.HangfireEndId);
            }
            var date = job.EndTime;
            job.HangfireEndId = BackgroundJob.Schedule(() => RecurringJob.RemoveIfExists(job.HangfireRecurringId), date);
            return job;
        }

        private HangfireJob CreateOrUpdateStartJob(HangfireJob job)
        {
            if (job.StartTime < DateTimeOffset.Now)
                return job;
            if (!string.IsNullOrEmpty(job.HangfireStartId))
            {
                RecurringJob.RemoveIfExists(job.HangfireStartId);
            }
            var date = job.StartTime;
            job.HangfireStartId = BackgroundJob.Schedule(() => CreateOrUpdateRecurringJob(job.Id), date);
            return job;
        }

        public void CreateOrUpdateRecurringJob(int dbJobId)
        {
            BaseJob job;
            var dbJob = Get<HangfireJob>(j => j.Id == dbJobId);
            switch ((JobType)dbJob.JobType)
            {
                case JobType.Renta:
                    job = new PeriodJob();
                    dbJob = CreateOrUpdateRecurringJob(dbJob, job);
                    break;
                case JobType.Test:
                    job = new TestJob();
                    dbJob = CreateOrUpdateRecurringJob(dbJob, job);
                    break;
                default:
                    throw new Exception("jobtype not found");
            }
            dbJob = CreateOrUpdateStopJob(dbJob);
            Add(dbJob);
            Context.SaveChanges();
        }

        private HangfireJob CreateOrUpdateRecurringJob(HangfireJob dbJob, BaseJob job)
        {
            if (string.IsNullOrEmpty(dbJob.HangfireRecurringId))
            {
                dbJob.HangfireRecurringId = Guid.NewGuid().ToString();
            }

            RecurringJob.AddOrUpdate(dbJob.HangfireRecurringId, () => job.DoJob(), $"{dbJob.Cron}");
            return dbJob;
        }

        #endregion
    }
}
