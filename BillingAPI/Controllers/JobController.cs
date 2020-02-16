using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Hangfire;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly IJobManager Manager = IocContainer.Get<IJobManager>();
        [HttpGet("getalljobs")]
        public ActionResult Index()
        {
            var jobs = Manager.GetAllJobs();
            return View(jobs);
        }

        [HttpPost("createnewjob")]
        public ActionResult CreateNewJob(HangfireJob newJob)
        {
            Manager.AddOrUpdateJob(newJob);
            return new JsonResult("ok");
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Edit()
        {
            return View();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Start()
        {
            return View();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Delete()
        {
            return View();
        }
    }
}