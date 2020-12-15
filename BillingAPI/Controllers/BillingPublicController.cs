using Billing;
using Billing.Dto;
using Billing.DTO;
using BillingAPI.Filters;
using BillingAPI.Model;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class BillingPublicController : EvarunApiController
    {
        /// <summary>
        /// Get base info for current character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpGet("getsin")]
        public DataResult<BalanceDto> GetSin(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(character), $"getsin {character}");
            return result;
        }

    }
}
