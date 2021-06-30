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

        [HttpPost("shop")]
        [ShopAuthorization]
        public Result UpdateTrustedShop(int shop, [FromBody] UpdateTrustedShopRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.UpdateShopTrustedUsers(request.Shop, request.TrustedModels));
            return result;
        }

        [HttpGet("organisations")]
        public DataResult<OrganisationViewModel> GetMyOrganisations(int character)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableOrganisations(character), $"character {character}");
            return result;
        }

        [HttpGet("corporationcontracts")]
        [CorporationAuthorization]
        public DataResult<List<ShopCorporationContractDto>> GetCorporationContracts(int corporation)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetCorporationContracts(corporation));
            return result;
        }

        [HttpGet("shopcontracts")]
        [ShopAuthorization]
        public DataResult<List<ShopCorporationContractDto>> GetShopContracts(int shop)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetShopContracts(shop));
            return result;
        }

        [HttpPost("suggestcontract")]
        [CorporationAuthorization]
        public Result SuggestContract([FromBody] SuggestContractRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.SuggestContract(request.Corporation, request.Shop));
            return result;
        }

        [HttpPost("proposecontract")]
        [CorporationAuthorization]
        public Result ProposeContract([FromBody] RequestTerminateContractRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.ProposeContract(request.Corporation, request.Shop));
            return result;
        }

        [HttpPost("approvecontract")]
        [ShopAuthorization]
        public Result ApproveContract([FromBody] ApproveContractRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.ApproveContract(request.Corporation, request.Shop));
            return result;
        }

        [HttpPost("terminatecontract")]
        [ShopAuthorization]
        public Result TerminateContract([FromBody] TerminateContractRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.TerminateContract(request.Corporation, request.Shop));
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
        public DataResult<List<QRDto>> GetProducts([FromBody] ShopBasedRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetAvailableQR(request.Shop), $"getproducts {request.Shop}");
            return result;
        }

        [HttpPost("getcorporationproducts")]
        [CorpAuthorization]
        public DataResult<List<SkuDto>> GetCorporationProducts([FromBody] CorporationBasedRequest request)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetSkus(s=>s.CorporationId == request.Corporation));
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
            var result = RunAction(() => manager.ConfirmRenta(request.Character, request.PriceId, request.Count), $"createrenta {request.Character}:{request.PriceId}:{request.Count}");
            return result;
        }
    }

}

