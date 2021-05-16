using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using BillingAPI.Filters;
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
        [HttpGet("cycle")]
        //[CheckSecret]
        public DataResult<string> ProcessCycle()
        {
            var result = RunAction(() => 
            {
                var life = new JobLifeService();
                return life.ToggleCycle();
            }, $"period");
            return result;
        }

        [HttpGet("beatcharacters")]
        //[CheckSecret]
        public DataResult<string> ProcessBeat()
        {
            var result = RunAction(() =>
            {
                var life = new JobLifeService();
                return life.DoBeat(BeatTypes.Characters);
            }, $"period");
            return result;
        }


    }
}