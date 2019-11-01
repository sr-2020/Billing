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
        BaseJob CreateJob(JobType type);
        List<Job> GetAllJobs();
    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        
        public BaseJob CreateJob(JobType type)
        {
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

        public BaseJob LoadJob(Job dbJob)
        {
            var job = CreateJob(dbJob.JType);

            return job;
        }

        public List<Job> GetAllJobs()
        {
            return Context.Jobs.ToList();
        }

    }
}
