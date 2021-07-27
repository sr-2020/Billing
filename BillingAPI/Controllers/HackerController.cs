using Billing;
using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using BillingAPI.Filters;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Controllers
{
    [Route("")]
    [Hacker]
    public class HackerController : EvarunApiAsyncController
    {
        [HttpPost("h-transfer")]
        public Result StealTransfer([FromBody] StealTransferRequest request)
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            manager.SaveHistory("h-transfer", request.From, request.To, $"amount = {request.Amount}, comment={request.Comment}");
            return RunAction(() => manager.StealMoney(request.From, request.To, request.Amount, request.Comment));
        }

        [HttpPost("h-shop-transfer")]
        public Result StealShopTransfer([FromBody] StealTransferRequest request)
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            manager.SaveHistory("h-shop-transfer", request.From, request.To, $"amount = {request.Amount}, comment={request.Comment}");
            return RunAction(() => manager.StealShopMoney(request.From, request.To, request.Amount, request.Comment));
        }

        [HttpPost("h-renta")]
        public Result StealRenta([FromBody] StealRentaRequest request)
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            manager.SaveHistory("h-renta", request.RentaId, request.To ?? 0, $"");
            return RunAction(() => manager.StealRenta(request.RentaId,request.To));
        }

        [HttpGet("h-shop")]
        public DataResult<ShopDetailedDto> GetShop(int shopId)
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            return RunAction(() => manager.GetHackerDetailedShop(shopId));
        }

        [HttpGet("h-shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            return RunAction(() => manager.GetHackerShops());
        }

        [HttpGet("h-corps")]
        public DataResult<List<CorporationDto>> GetCorps()
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            return RunAction(() => manager.GetHackerCorps());
        }

        [HttpPost("h-shop")]
        public Result HackShop([FromBody] HackShopRequest request)
        {
            var manager = IoC.IocContainer.Get<IHackerManager>();
            manager.SaveHistory("h-shop", request.ShopId, 0, string.Join(",", request.Models));
            return RunAction(() => manager.HackShop(request.ShopId, request.Models));
        }

        /// <summary>
        /// Get base info for current character
        /// </summary>
        [HttpGet("h-sin")]
        public DataResult<BalanceDto> GetSin(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(characterId), $"h-getsin {characterId}");
            return result;
        }

        /// <summary>
        /// Get all rentas for character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        [HttpGet("h-rentas")]
        public DataResult<RentaSumDto> GetRentas(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetRentas(characterId), $"getrentas {characterId}");
            return result;
        }

        /// <summary>
        ///  Get all transfers for character
        /// </summary>
        [HttpGet("h-transfers")]
        public async Task<DataResult<TransferSum>> GetTransfers(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = await RunActionAsync(() => manager.GetTransfersAsync(characterId), $"gettransfers {characterId}");
            return result;
        }

        /// <summary>
        ///  Get scoring for character
        /// </summary>
        [HttpGet("h-scoring")]
        public DataResult<ScoringDto> GetScoring(int characterId)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.GetFullScoring(characterId), $"get full scoring {characterId}");
        }

        /// <summary>
        ///  Get insolvents
        /// </summary>
        [HttpGet("i-insolvents")]
        public DataResult<List<InsolventDto>> GetInsolvents()
        {
            var manager = IocContainer.Get<IReadOnlyManager>();
            return RunAction(() => manager.GetInsolvents());
        }
    }
}
