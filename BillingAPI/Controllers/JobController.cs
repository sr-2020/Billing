using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using BillingAPI.Model;
using Core;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finished">if true, all jobs will be displayed, if false, then only not finished will be displaed</param>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("getalljobs")]
        public DataResult<List<HangfireJob>> Index(bool? finished, int jobType)
        {
            var result = RunAction(() => Manager.GetAllJobs(finished ?? true, jobType), $"getalljobs");
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start">yyyy-MM-dd HH:mm:ss</param>
        /// <param name="end">yyyy-MM-dd HH:mm:ss</param>
        /// <param name="cron"></param>
        /// <param name="jobname"></param>
        /// <param name="jobtype"></param>
        /// <returns></returns>
        [HttpPost("createorupdatejob")]
        public DataResult<HangfireJob> CreateOrUpdateJob(int id, string start, string end, string cron, string jobname, int jobtype)
        {
            var result = RunAction(() => Manager.AddOrUpdateJob(id, SystemHelper.SetMoscowTimeSpan(start), SystemHelper.SetMoscowTimeSpan(end), cron, jobname, jobtype), $"createorupdatejob");
            return result;
        }
        [HttpGet("getjobtypes")]
        public DataResult<IEnumerable<string>> GetJobTypes()
        {
            var result = RunAction(() => { return Enum.GetValues(typeof(JobType)).Cast<JobType>().Select(s => s.ToString()); }, $"GetJobTypes");
            return result;
        }

    }
}