using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto.Shop;
using Billing.DTO;
using BillingAPI.Filters;
using BillingAPI.Model;
using Core;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("shop")]
    [ApiController]
    public class ShopManagerController : EvarunApiController
    {
        [HttpGet("getmyshops")]
        public DataResult<ShopViewModel> GetMyShops(int character)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableShops(character), "getmyshops");
            return result;
        }

        [HttpPost("maketransfertosin")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegSIN(int shop, int sin, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(shop, sin, amount, comment));
            return result;
        }
        [HttpPost("maketransfertoleg")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegLeg(int shop, int leg2, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(shop, leg2, amount, comment));
            return result;
        }

        [HttpPost("getproducts")]
        [ShopAuthorization]
        public DataResult<List<SkuDto>> GetProducts(int shop)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSkusForShop(shop), $"getskuforshop {shop}");
            return result;
        }

    }

}

