using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing.Dto;
using BillingAPI.Model;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scoringspace;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoringController : EvarunApiController
    {

        [HttpGet("info/getmyscoring")]
        public DataResult<ScoringDto> GetScoring(int character)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(()=> manager.GetFullScoring(character), $"get full scoring {character}");
        }



    }
}