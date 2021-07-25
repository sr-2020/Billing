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
using IoC;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("[controller]")]
    [CheckSecret]
    public class JobController : EvarunApiController
    {
        [HttpGet("cycle")]
        public DataResult<string> ProcessCycle(string secret)
        {
            var result = RunAction(() => 
            {
                var life = new JobLifeService();
                return life.ToggleCycle();
            }, $"period");
            return result;
        }

        [HttpGet("beatcharacters")]
        public DataResult<string> ProcessBeat()
        {
            var result = RunAction(() =>
            {
                var life = new JobLifeService();
                return life.DoBeat(BeatTypes.Characters);
            }, $"beatcharacters");
            return result;
        }

        [HttpGet("beatitems")]
        public DataResult<string> ProcessBeatItems()
        {
            var result = RunAction(() =>
            {
                var life = new JobLifeService();
                return life.DoBeat(BeatTypes.Items);
            }, $"beatitems");
            return result;
        }


    }
}