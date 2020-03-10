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
        /// Get corporation wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="corporationId">any id</param>
        /// <param name="amount">if negative value then amount will no change or 0 </param>
        /// <returns></returns>
        [HttpGet("admin/CreateOrUpdateCorporationWallet")]
        public DataResult<CorporationWallet> CreateOrUpdateCorporationWallet(int corporationId, decimal amount)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateCorporationWallet(corporationId, amount));
            return result;
        }
        /// <summary>
        /// Get shop wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="shopId">any id</param>
        /// <param name="amount">if negative value then amount will no change or 0</param>
        /// <returns></returns>
        [HttpGet("admin/CreateOrUpdateShopWallet")]
        public DataResult<ShopWallet> CreateOrUpdateShopWallet(int shopId, decimal amount)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(shopId, amount));
            return result;
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
        /// <summary>
        /// Create personal price to current productType for current character
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="corporation"></param>
        /// <param name="shop"></param>
        /// <param name="character"></param>
        /// <param name="basePrice"></param>
        /// <param name="shopComission"></param>
        /// <returns></returns>
        [HttpGet("renta/createprice")]
        public DataResult<PriceDto> CreatePrice(int productType, int corporation, int shop, int character, decimal basePrice, decimal shopComission = 0)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPrice(productType, corporation, shop, character, basePrice, shopComission));
            return result;
        }
        /// <summary>
        /// Create renta
        /// </summary>
        /// <param name="priceId">personal price created on api/billing/renta/createprice</param>
        /// <returns></returns>
        [HttpGet("renta/createrenta ")]
        public DataResult<Renta> ConfirmRenta(int priceId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.ConfirmRenta(priceId));
            return result;
        }
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
            var result = RunAction(() => manager.GetSinStringByCharacter(characterId));
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