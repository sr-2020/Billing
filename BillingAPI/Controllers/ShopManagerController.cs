using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto;
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
        #region refactored
        
        [HttpGet("getmyorganisations")]
        public DataResult<OrganisationViewModel> GetMyOrganisations(int character)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableOrganisations(character), "getmyorganisations");
            return result;
        }

        #endregion

        [HttpGet("getmyshops")]
        [Obsolete]
        public DataResult<OrganisationViewModel> GetMyShops(int character)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableOrganisations(character), "getmyshops");
            return result;
        }

        [HttpPost("maketransfertosin")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegSIN([FromBody] MakeTransferLegSINRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.MakeTransferLegSIN(request.Shop, request.Sin, request.Amount, request.Comment), "maketransfertosin");
            return result;
        }

        [HttpPost("maketransfertoleg")]
        [ShopAuthorization]
        public DataResult<Transfer> MakeTransferLegLeg([FromBody] MakeTransferLegLegRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.MakeTransferLegLeg(request.Shop, request.ShopTo, request.Amount, request.Comment), "maketransfertoleg");
            return result;
        }

        [HttpPost("gettransfers")]
        [ShopAuthorization]
        public DataResult<List<TransferDto>> GetTranfers([FromBody] GetTranfersRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetTransfers(request.Shop), $"gettransfers {request.Shop}");
            return result;
        }

        [HttpPost("getproducts")]
        [ShopAuthorization]
        public DataResult<List<QRDto>> GetProducts([FromBody] GetProductsRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableQR(request.Shop), $"getproducts {request.Shop}");
            return result;
        }

        [HttpPost("getrentas")]
        [ShopAuthorization]
        public DataResult<List<RentaDto>> GetRentas([FromBody] GetRentasRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetRentas(request.Shop), $"getrentas {request.Shop}");
            return result;
        }

        [HttpPost("writerenta2qr")]
        public Result WriteOffer([FromBody] WriteOfferRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.WriteRenta(request.RentaId, request.Qr), $"writerenta2qr {request.RentaId}:{request.Qr}");
            return result;
        }

        [HttpPost("createpricebyqr")]
        public DataResult<PriceShopDto> GetPriceByQR([FromBody] GetPriceByQRRequest request) 
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPriceByQR(request.Character, request.Qr), $"createpricebyqr {request.Character}:{request.Qr}");
            return result;
        }

        [HttpPost("createrenta")]
        public DataResult<RentaDto> CreateRenta([FromBody] CreateRentaRequest request)  
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.ConfirmRenta(request.Character, request.PriceId), $"createrenta {request.Character}:{request.PriceId}");
            return result;
        }
    }

}

