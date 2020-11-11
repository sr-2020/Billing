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
    [Route("[controller]")]
    public class JobController : EvarunApiController
    {
        private readonly IJobManager Manager = IocContainer.Get<IJobManager>();

        [HttpGet("period")]
        public Result ProcessPeriod()
        {
            var result = RunAction(() => Manager.ProcessPeriod(), $"period");
            return result;
        }

        [HttpGet("getalljobs")]
        public DataResult<List<HangfireJob>> Index(bool? finished, int jobType)
        {
            throw new NotImplementedException();
            var result = RunAction(() => Manager.GetAllJobs(finished ?? true, jobType), $"getalljobs");
            return result;
        }


    }
}