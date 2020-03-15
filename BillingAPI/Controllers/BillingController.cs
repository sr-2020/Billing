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
        /// <param name="characterId">ID from table Character</param>
        /// <param name="balance">initial wallet amount</param>
        /// <returns></returns>
        [HttpGet("admin/createphysicalwallet")]
        public DataResult<SIN> CreatePhysicalWallet(int characterId, decimal balance)
        {
            var manager = IocContainer.Get<IBillingManager>(); 
            var result = RunAction(() => manager.CreateOrUpdatePhysicalWallet(characterId, balance), $"createphysicalwallet {characterId} {balance}");
            return result;
        }

        /// <summary>
        /// Create or update allowed product type
        /// </summary>
        /// <param name="code">unique code</param>
        /// <param name="name">shot description</param>
        /// <param name="description">full description</param>
        /// <param name="lifestyle">lifestyle, from 1 to 6</param>
        /// <param name="basePrice">recommended price</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdateproduct")]
        public DataResult<ProductType> CreateOrUpdateProductType(string code, string name, string description, int lifestyle, int basePrice)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateProductType(code, name, description, lifestyle, basePrice), $"createorupdateproduct {code} {name} {lifestyle} {basePrice}");
            return result;
        }

        /// <summary>
        /// Get corporation wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="foreignId">Some unique id, set 0 if u want to autogenerate it</param>
        /// <param name="amount">if negative then amount will not change</param>
        /// <param name="name">Some name</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdatecorporationwallet")]
        public DataResult<CorporationWallet> CreateOrUpdateCorporationWallet(int foreignId, decimal amount, string name)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateCorporationWallet(foreignId, amount, name), $"createorupdatecorporationwallet {foreignId} {amount} {name}");
            return result;
        }

        /// <summary>
        /// Get shop wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="foreignId">Some unique id, set 0 if u want to autogenerate it</param>
        /// <param name="amount">if negative value then amount will no change or 0</param>
        /// <param name="name">Some name</param>
        /// <param name="comission">Some name</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdateshopwallet")]
        public DataResult<ShopWallet> CreateOrUpdateShopWallet(int foreignId, decimal amount, string name, int comission)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(foreignId, amount, name, comission), $"createorupdateshopwallet {foreignId} {amount} {name} {comission}");
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
        /// <param name="comission"></param>
        /// <returns></returns>
        [HttpPost("renta/createprice")]
        public DataResult<PriceDto> CreatePrice(int productType, int corporation, int shop, int character, decimal basePrice, int comission)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPrice(productType, corporation, shop, character, basePrice, comission));
            return result;
        }

        [HttpPost("renta/createprice")]
        public DataResult<PriceDto> CreatePrice(int skuId, int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPrice(skuId, character));
            return result;
        }

        /// <summary>
        /// Create renta
        /// </summary>
        /// <param name="priceId">personal price created on api/billing/renta/createprice</param>
        /// <returns></returns>
        [HttpPost("renta/createrenta ")]
        public DataResult<Renta> ConfirmRenta(int priceId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.ConfirmRenta(priceId));
            return result;
        }
        #endregion

        #region info
        [HttpGet("info/getrentas")]
        public DataResult<List<RentaDto>> GetRentas(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetRentas(characterId), "getrentas");
            return result;
        }

        /// <summary>
        /// Get all shops
        /// </summary>
        /// <returns>list of</returns>
        [HttpGet("info/getshops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetShops(), "getshops");
            return result;
        }

        [HttpGet("info/getcorps")]
        public DataResult<List<CorporationDto>> GetCorps()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetCorps(), "getcorps");
            return result;
        }

        [HttpGet("info/getproducttypes")]
        public DataResult<List<ProductTypeDto>> GetProductTypes()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetProductTypes(), "getproducttypes");
            return result;
        }

        /// <summary>
        /// Get base info for current character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        [HttpGet("info/getbalance")]
        public DataResult<BalanceDto> GetBalance(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetBalance(characterId), $"getbalance for {characterId}");
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
            var result = RunAction(() => manager.GetTransfers(characterId), $"gettransfers for {characterId}");
            return result;
        }

        #endregion
    }
}