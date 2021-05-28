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
        [HttpGet("sin")]
        public DataResult<BalanceDto> GetSin(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(character), $"getsin {character}");
            return result;
        }

        /// <summary>
        /// Get all rentas for character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpGet("rentas")]
        public DataResult<RentaSumDto> GetRentas(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetRentas(character), $"getrentas {character}");
            return result;
        }

        /// <summary>
        ///  Get all transfers for character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpGet("transfers")]
        public DataResult<TransferSum> GetTransfers(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetTransfers(character), $"gettransfers {character}");
            return result;
        }

        /// <summary>
        /// Create transfer from Character1 to Character2 using sins
        /// </summary>
        /// <returns></returns>
        [HttpPost("createtransfersinsin")]
        public DataResult<Transfer> CreateTransferSINSIN(int character, [FromBody] CreateTransferSinSinRequest request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            DataResult<Transfer> result;
            if (string.IsNullOrEmpty(request.SinTo))
            {
                result = RunAction(() => manager.MakeTransferSINSIN(character, request.CharacterTo, request.Amount, request.Comment), "transfer/createtransfersinsin");
            }
            else
            {
                result = RunAction(() => manager.MakeTransferSINSIN(character, request.SinTo, request.Amount, request.Comment), $"transfer/createtransfersinsin {character}=>{request.SinTo}:{request.Amount}");
            }

            return result;
        }

    }
}
