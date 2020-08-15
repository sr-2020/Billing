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
    [Route("api/[controller]")]
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
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.MakeTransferLegSIN(shop, sin, amount, comment), "maketransfertosin");
            return result;
        }

        [HttpPost("maketransfertoleg")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegLeg(int shop, int shopTo, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.MakeTransferLegLeg(shop, shopTo, amount, comment), "maketransfertoleg");
            return result;
        }

        [HttpPost("gettransfers")]
        [ShopAuthorization]
        public DataResult<List<TransferDto>> GetTranfers(int shop)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetTransfers(shop), $"gettransfers {shop}");
            return result;
        }

        [HttpPost("getproducts")]
        [ShopAuthorization]
        public DataResult<List<QRDto>> GetProducts(int shop)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableQR(shop), $"getproducts {shop}");
            return result;
        }

        [HttpPost("getrentas")]
        [ShopAuthorization]
        public DataResult<List<RentaDto>> GetRentas(int shop)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetRentas(shop), $"getrentas {shop}");
            return result;
        }

        [HttpPost("writerenta2qr")]
        public Result WriteOffer(int rentaId, string qr)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.WriteRenta(rentaId, qr), $"writerenta2qr {rentaId}:{qr}");
            return result;
        }
    }

}

