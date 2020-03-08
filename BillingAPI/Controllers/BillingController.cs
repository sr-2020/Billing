using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.DTO;
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

        #region admin
        /// <summary>
        /// Fill all tables related with wallet for current character
        /// </summary>
        /// <param name="character">ID from table Character</param>
        /// <param name="balance">initial wallet amount</param>
        /// <returns></returns>
        [HttpGet("admin/createphysicalwallet")]
        public DataResult<SIN> CreatePhysicalWallet(int character, decimal balance)
        {
            var manager = IocContainer.Get<IBillingManager>(); 
            var result = RunAction(() => manager.CreateOrUpdatePhysicalWallet(character, balance));
            return result;
        }
        /// <summary>
        /// Create or update allowed product type
        /// </summary>
        /// <param name="code">unique code</param>
        /// <param name="name">shot description</param>
        /// <param name="description">full description</param>
        /// <returns></returns>
        [HttpGet("admin/createorupdateproduct")]
        public DataResult<ProductType> CreateOrUpdateProductType(string code, string name, string description)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateProductType(code, name, description));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corporationId">any id</param>
        /// <returns></returns>
        [HttpGet("admin/CreateOrUpdateCorporation")]
        public DataResult<CorporationWallet> CreateOrUpdateCorporation(int corporationId)
        {
            throw new Exception("123");
        }

        public DataResult<ShopWallet> CreateOrUpdateShop(int soptId)
        {
            throw new Exception("123");
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
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINSIN(character1, character2, amount, comment));
            return result;
        }
        [HttpGet("transfer/maketransfersinleg")]
        public DataResult<Transfer> MakeTransferSINLeg(int sin, int leg, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(sin, leg, amount, comment));
            return result;
        }
        [HttpGet("transfer/maketransferlegsin")]
        public DataResult<Transfer> MakeTransferLegSIN(int leg, int sin, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(leg, sin, amount, comment));
            return result;
        }
        [HttpGet("transfer/maketransferlegleg")]
        public DataResult<Transfer> MakeTransferLegLeg(int leg1, int leg2, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINLeg(leg1, leg2, amount, comment));
            return result;
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

        /// <summary>
        /// Get base info for current character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        [HttpGet("info/getbalance")]
        public DataResult<BalanceDto> GetBalance(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(characterId));
            return result;
        }

        [HttpGet("info/getcharacteridbysin")]
        public DataResult<int> GetCharacterIdBySin(string sinString)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetCharacterIdBySin(sinString));
            return result;
        }

        [HttpGet("info/getsinbycharacterId")]
        public DataResult<string> GetSinByCharacter(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSinByCharacter(characterId));
            return result;
        }
        /// <summary>
        /// Get all transfers(income and outcome) for current character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        [HttpGet("info/gettransfers")]
        public DataResult<List<TransferDto>> GetTransfers(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetTransfers(characterId));
            return result;
        }

        #endregion
    }
}