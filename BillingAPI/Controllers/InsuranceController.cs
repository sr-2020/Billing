using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InsuranceController : EvarunApiController
    {
        [HttpGet("getinsurance")]
        public DataResult<InsuranceDto> GetInsurance(int characterId)
        {
            var manager = IocContainer.Get<IInsuranceManager>();
            var result = RunAction(() => manager.GetInsurance(characterId), $"GetInsurance({characterId})"); ;
            return result;
        }
    }
}
