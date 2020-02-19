using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using BillingAPI.Model;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : EvarunApiController
    {
        //private readonly Lazy<IBillingManager> _manager = new Lazy<IBillingManager>(IocContainer.Get<IBillingManager>);

        #region admin
        /// <summary>
        /// Fill all tables related with wallet for current character
        /// </summary>
        /// <param name="character">ID from table Character</param>
        /// <param name="balance">initial wallet amount</param>
        /// <returns></returns>
        [HttpGet("admin/createphysicalwallet")]
        public DataResult<SINDetails> CreatePhysicalWallet(int character, decimal balance)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.CreatePhysicalWallet(character, balance));
                return result;
            }
        }
        #endregion

        #region transfer
        /// <summary>
        /// Create transfer from Character1 to Character2 using sins
        /// </summary>
        /// <param name="character1">ID from table Character</param>
        /// <param name="character2">ID from table Character</param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpGet("transfer/maketransfersinsin")]
        public DataResult<Transfer> MakeTransferSINSIN(int character1, int character2, decimal amount, string comment)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.MakeTransferSINSIN(character1, character2, amount, comment));
                return result;
            }
        }
        [HttpGet("transfer/maketransfersinleg")]
        public DataResult<Transfer> MakeTransferSINLeg(int sin, int leg, decimal amount, string comment)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.MakeTransferSINLeg(sin, leg, amount, comment));
                return result;
            }
        }
        [HttpGet("transfer/maketransferlegsin")]
        public DataResult<Transfer> MakeTransferLegSIN(int leg, int sin, decimal amount, string comment)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.MakeTransferSINLeg(leg, sin, amount, comment));
                return result;
            }
        }
        [HttpGet("transfer/maketransferlegleg")]
        public DataResult<Transfer> MakeTransferLegLeg(int leg1, int leg2, decimal amount, string comment)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.MakeTransferSINLeg(leg1, leg2, amount, comment));
                return result;
            }
        }
        #endregion
        #region renta
        //[HttpGet("renta/createprice")]
        //public Result CreatePrice()
        //{
        //    throw new NotImplementedException();
        //}
        //[HttpGet("renta/createpricebyparams ")]
        //public Result CreatePriceByParams()
        //{
        //    throw new NotImplementedException();
        //}
        //[HttpGet("renta/confirmrenta ")]
        //public Result ConfirmRenta()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
        #region info
        [HttpGet("info/getcharacteridbysin")]
        public DataResult<int> GetCharacterIdBySin(string sinString)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.GetCharacterIdBySin(sinString));
                return result;
            }
        }

        [HttpGet("info/getsinbycharacterId")]
        public DataResult<string> GetSinByCharacter(int characterId)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.GetSinByCharacter(characterId));
                return result;
            }
        }
        /// <summary>
        /// Get all transfers(senders and recipients) and  for current character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        [HttpGet("info/gettransfers")]
        public DataResult<List<Transfer>> GetTransfers(int characterId)
        {
            using (var manager = new BillingManager())
            {
                var result = RunAction(() => manager.GetTransfers(characterId));
                return result;
            }
        }

        #endregion
    }
}