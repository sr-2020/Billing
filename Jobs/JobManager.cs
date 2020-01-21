using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jobs
{
    public interface IJobManager : IBaseRepository
    {
        HangfireJob AddOrUpdateJob(HangfireJob hangfire);
        List<HangfireJob> GetAllJobs();
    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        public HangfireJob AddOrUpdateJob(HangfireJob hangfire)
        {
            HangfireJob exists;
            if (hangfire.Id == 0)
            {
                exists = Context.Job.FirstOrDefault(j => j.JobName == hangfire.JobName);
                if (exists != null)
                    throw new Exception($"{hangfire.JobName} already exists");
            }
            else
            {
                
            }


            switch (type)
            {
                case JobType.Test:
                    return new TestJob();
                case JobType.Scoring:
                    return new ScoringJob();
                case JobType.Profits:
                    return new ProfitsJob();
                case JobType.Credits:
                    return new CreditsJob();

                default:
                    break;
            }
            throw new NotImplementedException("Job not configured");
        }

        public BaseJob LoadJob(HangfireJob dbJob)
        {
            var job = CreateJob(dbJob.JobType);

            return job;
        }

        public List<HangfireJob> GetAllJobs()
        {
            return Context.Job.ToList();
        }

    }
}
