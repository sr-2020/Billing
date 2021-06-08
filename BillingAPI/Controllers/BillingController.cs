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
        #region refactored

        [HttpGet("getshop")]
        [AdminAuthorization]
        public DataResult<ShopDto> GetShop(int shopId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetShops(s => s.Id == shopId).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getcharacters")]
        [AdminAuthorization]
        public DataResult<List<CharacterDto>> GetCharacters()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetCharactersInGame(), $"getcharacters ");
            return result;
        }

        #endregion

        #region admin

        /// <summary>
        /// Fill all tables related with wallet for current character
        /// </summary>
        /// <param name="character">ID from table Character</param>
        /// <param name="balance">initial wallet amount</param>
        /// <returns></returns>
        [HttpGet("admin/createphysicalwallet")]
        [Obsolete]
        public DataResult<SIN> CreatePhysicalWallet(int character, decimal balance)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdatePhysicalWallet(character, balance), $"createphysicalwallet {character} {balance}");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelid"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("admin/initcharacter/{modelid}")]
        public DataResult<SIN> InitCharacter(int modelid, [FromBody] InitCharacterRequest request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.InitCharacter(modelid), $"InitCharacter: {modelid}");
            return result;
        }
        [HttpPost("createtransfermir")]
        [AdminAuthorization]
        public DataResult<Transfer> CreateTransferMIR([FromBody] CreateTransferSinSinRequest request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateTransferMIRSIN(request.CharacterTo, request.Amount), "createtransfermir");
            return result;
        }

        ///// <summary>
        ///// Get corporation wallet. If wallet not exists, then create it
        ///// </summary>
        ///// <param name="id">0 for create new, specified for update</param>
        ///// <param name="amount">if negative then amount will not change</param>
        ///// <param name="name">Some name</param>
        ///// <param name="logoUrl">Url to picture</param>
        ///// <returns></returns>
        //[HttpPut("admin/createorupdatecorporationwallet")]
        //[AdminAuthorization]
        //public DataResult<CorporationWallet> CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl)
        //{
        //    var manager = IocContainer.Get<IAdminManager>();
        //    var result = RunAction(() => manager.CreateOrUpdateCorporationWallet(id, amount, name, logoUrl), $"createorupdatecorporationwallet {id}:{amount}:{name}:{logoUrl}");
        //    return result;
        //}

        #endregion

        #region transfer

        [Obsolete]
        [HttpGet("transfer/maketransfersinsin")]
        public DataResult<Transfer> MakeTransferSINSIN(int character1, int character2, decimal amount, string comment)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.MakeTransferSINSIN(character1, character2, amount, comment), "transfer/maketransfersinsin");
            return result;
        }

        /// <summary>
        /// Create transfer from Character1 to Character2 using sins
        /// </summary>
        /// <returns></returns>
        [HttpPost("transfer/createtransfersinsin")]
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

        #endregion

        #region renta

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
        public DataResult<List<SkuDto>> GetSkus(int corporationId, int? nomenklaturaId, bool? enabled)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSkus(corporationId, nomenklaturaId ?? 0, enabled), $"getskus {corporationId}:{nomenklaturaId ?? 0}:{enabled}");
            return result;
        }

        /// <summary>
        /// Get all rentas for current character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        [HttpGet("info/getrentas")]
        public DataResult<List<RentaDto>> GetRentas(int character)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetRentas(character)?.Rentas, $"getrentas {character}");
            return result;
        }

        /// <summary>
        /// Get all corporations
        /// </summary>
        /// <returns></returns>
        [HttpGet("info/getcorps")]
        public DataResult<List<CorporationDto>> GetCorps()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetCorporations(s => true), "getcorps");
            return result;
        }

        [HttpGet("info/getcharacteridbysin")]
        public DataResult<int> GetCharacterIdBySin(string sinString)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetModelIdBySinString(sinString), "info/getcharacteridbysin");
            return result;
        }

        [HttpGet("info/getsinbycharacterId")]
        public DataResult<string> GetSinByCharacter(int characterId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.GetSinStringByCharacter(characterId), "info/getsinbycharacterId");
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
            var result = RunAction(() => manager.GetTransfers(characterId)?.Transfers, $"gettransfers {characterId}");
            return result;
        }

        #endregion
    }
}