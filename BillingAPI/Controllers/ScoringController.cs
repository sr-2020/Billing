using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoringController : ControllerBase
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

        [HttpGet("info/getscoring")]
        public DataResult<Scoring> GetScoring(int characterId)
        {
            var orc = new Scoring();
            orc.CurrentScoring = 0;
            //orc.Calculates = new List<ScoringFactorCalculate>();
            //foreach (var item in collection)
            //{

            //}

            throw new NotImplementedException();
            //var manager = _manager.Value;
            //var result = RunAction(() => manager.GetTransfers(characterId));
            //return result;
        }

    }
}