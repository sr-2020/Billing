using Billing;
using Billing.DTO;
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
        /// Create transfer from Character1 to Character2 using sins
        /// </summary>
        /// <returns></returns>
        [HttpPost("createtransfer")]
        public DataResult<Transfer> CreateTransferSINSIN(int character, [FromBody] CreateTransferSinSinRequest request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateTransferSINSIN(character.ToString(), request.CharacterTo, request.Amount, request.Comment));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpGet("gettransfers")]
        public DataResult<List<TransferDto>> GetTransfers(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetTransfers(character), $"gettransfers for {character}");
            return result;
        }
    }
}
