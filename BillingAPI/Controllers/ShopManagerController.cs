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
            var result = RunAction(() => manager.MakeTransferSINLeg(shop, sin, amount, comment), "maketransfertosin");
            return result;
        }
        [HttpPost("maketransfertoleg")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegLeg(int shop, int shopTo, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(shop, shopTo, amount, comment), "maketransfertoleg");
            return result;
        }

        [HttpPost("gettransfers")]
        [ShopAuthorization]
        public DataResult<List<TransferDto>> GetTranfers(int shop)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetTransfers(shop), "gettransfers");
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

