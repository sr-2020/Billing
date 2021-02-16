using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.DTO;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class DashboardController : EvarunApiController
    {

        /// <summary>
        /// GetShops
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetShops(s => true));
            return result;
        }
    }
}
