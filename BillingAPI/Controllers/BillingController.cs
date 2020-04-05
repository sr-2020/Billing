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
            var result = RunAction(() => manager.CreateOrUpdatePhysicalWallet(character, balance), $"createphysicalwallet {character} {balance}");
            return result;
        }

        /// <summary>
        /// Create or update allowed product type
        /// </summary>
        /// <param name="id">0 for create new, specified for update</param>
        /// <param name="name">short description</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdateproduct")]
        public DataResult<ProductType> CreateOrUpdateProductType(int id, string name)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateProductType(id, name), $"createorupdateproduct {id} {name}");
            return result;
        }

        /// <summary>
        /// Create or update allowed nomenklatura
        /// </summary>
        /// <param name="id">0 for create new, specified for update</param>
        /// <param name="name">Header of product</param>
        /// <param name="code">it will be executed when user byu sku of this nomenklatura</param>
        /// <param name="producttype">id from getproducttypes</param>
        /// <param name="lifestyle">from 1 to 6</param>
        /// <param name="baseprice">decimal base price</param>
        /// <param name="description">description shown for user</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdatenomenklatura")]
        public DataResult<Nomenklatura> CreateOrUpdateNomenklatura(int id, string name, string code, int producttype, int lifestyle, decimal baseprice, string description)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateNomenklatura(id, name, code, producttype, lifestyle, baseprice, description), $"createorupdatenomenklatura {id}:{name}:{code}:{producttype}:{lifestyle}:{baseprice}:{description}");
            return result;
        }

        /// <summary>
        /// Create or update allowed sku
        /// </summary>
        /// <param name="id">0 for create new, specified for update</param>
        /// <param name="nomenklatura">id from getnomenklaturas</param>
        /// <param name="count">count of this item, minimum 1</param>
        /// <param name="corporation">id from getcorps</param>
        /// <param name="name">header</param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpPut("admin/createorupdatesku")]
        public DataResult<Sku> CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateSku(id, nomenklatura, count, corporation, name, enabled), $"CreateOrUpdateSku {id}:{name}:{nomenklatura}:{count}:{corporation}:{name}:{enabled}");
            return result;
        }

        /// <summary>
        /// Get corporation wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="id">0 for create new, specified for update</param>
        /// <param name="amount">if negative then amount will not change</param>
        /// <param name="name">Some name</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdatecorporationwallet")]
        public DataResult<CorporationWallet> CreateOrUpdateCorporationWallet(int id, decimal amount, string name)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateCorporationWallet(id, amount, name), $"createorupdatecorporationwallet {id} {amount} {name}");
            return result;
        }

        /// <summary>
        /// Get shop wallet. If wallet not exists, then create it
        /// </summary>
        /// <param name="foreignId">0 for create new, specified for update</param>
        /// <param name="amount">if negative value then amount will no change or 0</param>
        /// <param name="name">Some name</param>
        /// <param name="lifestyle">lifestyle from 1 to 6</param>
        /// <returns></returns>
        [HttpPut("admin/createorupdateshopwallet")]
        public DataResult<ShopWallet> CreateOrUpdateShopWallet(int foreignId, decimal amount, string name, int lifestyle)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(foreignId, amount, name, lifestyle), $"createorupdateshopwallet {foreignId} {amount} {name} {lifestyle}");
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
        [HttpPost("renta/writeqr")]
        public DataResult<ShopQR> WriteQR(int qr, int shop, int sku)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.WriteQR(qr, shop, sku), $"writeqr {qr}:{shop}:{sku}");
            return result;
        }

        [HttpPost("renta/writefreeqr")]
        public DataResult<ShopQR> WriteFreeQR(int shop, int sku)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.WriteFreeQR(shop, sku), $"writefreeqr {shop} {sku}");
            return result;
        }

        [HttpDelete("renta/dropqr")]
        public DataResult<ShopQR> CleanQR(int qr)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CleanQR(qr), $"dropqr {qr}");
            return result;
        }

        [HttpPost("renta/createcontract")]
        public DataResult<Contract> CreateContract(int corporation, int shop)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateContract(corporation, shop), $"CreateContract {corporation} {shop}");
            return result;
        }

        [HttpDelete("renta/breakcontract")]
        public Result BreakContract(int corporation, int shop)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.BreakContract(corporation, shop), $"BreakContract {corporation} {shop}");
            return result;
        }

        [HttpPost("renta/createprice")]
        public DataResult<PriceDto> GetPriceByShop(int character, int shop, int sku)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPrice(shop, character, sku), $"createprice {character}:{shop}:{sku}");
            return result;
        }

        [HttpPost("renta/createpricebyqr")]
        public DataResult<PriceDto> GetPriceByQR(int character, int qr)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetPriceByQR(character, qr), $"createpricebyqr {character}:{qr}");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="priceId"></param>
        /// <returns></returns>
        [HttpPost("renta/createrenta")]
        public DataResult<Renta> ConfirmRenta(int character, int priceId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.ConfirmRenta(character, priceId), $"createrenta {character}:{priceId}");
            return result;
        }
        #endregion

        #region info

        [HttpGet("info/getcontracts")]
        public DataResult<List<Contract>> GetContrats(int shopid, int corporationId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetContrats(shopid, corporationId), $"GetContrats {shopid}:{corporationId}");
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="corporationId"></param>
        /// <param name="nomenklaturaId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpGet("info/getskus")]
        public DataResult<List<SkuDto>> GetSkus(int corporationId, int nomenklaturaId, bool? enabled)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSkus(corporationId, nomenklaturaId, enabled), $"getskus {corporationId}:{nomenklaturaId}:{enabled}");
            return result;
        }

        /// <summary>
        /// Get all allowed sku for current shop
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpGet("info/getskuforshop")]
        public DataResult<List<SkuDto>> GetSkusForShop(int shop)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSkusForShop(shop), $"getskuforshop {shop}");
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="producttype"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        [HttpGet("info/getnomenklaturas")]
        public DataResult<List<NomenklaturaDto>> GetNomenklaturas(int producttype, int lifestyle)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetNomenklaturas(producttype, lifestyle), $"getnomenklaturas {producttype}");
            return result;
        }

        /// <summary>
        /// Get all rentas for current character
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get all corporations
        /// </summary>
        /// <returns></returns>
        [HttpGet("info/getcorps")]
        public DataResult<List<CorporationDto>> GetCorps()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetCorps(), "getcorps");
            return result;
        }
        /// <summary>
        /// get all product types
        /// </summary>
        /// <returns></returns>
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