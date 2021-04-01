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
        [HttpGet("test")]
        public DataResult<Scoring> Test(int id)
        {
            //var orc = new Scoring();
            //orc.CurrentScoring = 0;
            //orc.CategoryCalculates = new List<ScoringCategoryCalculate>();
            //orc.FactorCalculates = new List<ScoringFactorCalculate>();
            //foreach (var item in orc.CategoryCalculates)
            //{
            //    item.Calculate(orc.FactorCalculates);
            //}
            //orc.CurrentScoring = orc.CategoryCalculates.Sum(c => c.Current * c.Category.Weight);

            throw new NotImplementedException();
            //var manager = _manager.Value;
            //var result = RunAction(() => manager.GetTransfers(characterId));
            //return result;
        }

        [HttpGet("info/getmyscoring")]
        public DataResult<ScoringDto> GetScoring(int character)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(()=> manager.GetFullScoring(character));
        }

    }
}