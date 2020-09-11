using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto.Shop;
using Billing.DTO;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("info")]
    [ApiController]
    public class InfoController : EvarunApiController
    {
        [HttpGet("getbalanсe")]
        public DataResult<BalanceDto> GetBalanсe(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(character), "getbalanсe");
            return result;
        }
    }
}
