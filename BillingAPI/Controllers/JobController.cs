using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Core.Model;
using Core.Primitives;
using Hangfire;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    public class JobController : EvarunApiController
    {
        private readonly IJobManager Manager = IocContainer.Get<IJobManager>();
        [HttpGet("getalljobs")]
        public DataResult<List<HangfireJob>> Index()
        {
            var result = RunAction(() => Manager.GetAllJobs(), $"getalljobs");
            return result;
        }

        [HttpPost("createorupdatejob")]
        public DataResult<HangfireJob> CreateOrUpdateJob(int id, DateTime? start, DateTime? end, string cron, string jobname, int jobtype)
        {
            var result = RunAction(() => Manager.AddOrUpdateJob(id, start ?? DateTime.MinValue, end ?? DateTime.MinValue, cron, jobname, jobtype), $"createorupdatejob");
            return result;
        }
        [HttpGet("getjobtypes")]
        public DataResult<IEnumerable<JobType>> GetJobTypes()
        {
            var result = RunAction(() => { return Enum.GetValues(typeof(JobType)).Cast<JobType>(); }, $"GetJobTypes");
            return result;
        }

    }
}