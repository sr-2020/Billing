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
        HangfireJob AddOrUpdateJob(int id, DateTimeOffset? start, DateTimeOffset? end, string cron, string jobname, int jobtype);
        List<HangfireJob> GetAllJobs();
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

        public List<HangfireJob> GetAllJobs()
        {
            return Context.Job.ToList();
        }

        private HangfireJob CreateOrUpdateStopJob(HangfireJob job)
        {
            if (!string.IsNullOrEmpty(job.HangfireEndId))
            {
                RecurringJob.RemoveIfExists(job.HangfireEndId);
            }
            var date = job.EndTime;
            job.HangfireEndId = BackgroundJob.Schedule(() => RecurringJob.RemoveIfExists(job.HangfireReccurentId), date);
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
            job.HangfireStartId = BackgroundJob.Schedule(() => CreateOrUpdateRecurringJob((JobType)job.JobType, job.Id), date);
            return job;
        }

        public void CreateOrUpdateRecurringJob(JobType jobtype, int dbJobId)
        {
            BaseJob job;
            var dbJob = Get<HangfireJob>(j => j.Id == dbJobId);
            switch (jobtype)
            {
                case JobType.Renta:
                    job = new RentaJob();
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
            if (string.IsNullOrEmpty(dbJob.HangfireReccurentId))
            {
                dbJob.HangfireReccurentId = Guid.NewGuid().ToString();
            }
            
            RecurringJob.AddOrUpdate(dbJob.HangfireReccurentId, () => job.DoJob(), $"{dbJob.Cron}");
            return dbJob;
        }
    }
}
