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
    public class JobController : Controller
    {
        private readonly IJobManager Manager = IocContainer.Get<IJobManager>();

        public ActionResult Index()
        {
            var jobs = Manager.GetAllJobs();
            return View(jobs);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Start()
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }
    }
}