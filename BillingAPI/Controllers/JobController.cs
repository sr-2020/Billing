using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("run/{id}")]
        public string Run(string id)
        {
            try
            {
                var jm = IocContainer.Get<IJobManager>();
                var config = jm.GetConfig(id);
                var job = jm.GetJob(config.Name);
                job.ScheduleJob(id, config.StartTime, config.EndTime, config.IntervalInMinutes);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return "success";
        }

        [HttpGet("stop/{id}")]
        public string Stop(string id)
        {
            try
            {
                //TODO
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return "success";
        }
    }
}