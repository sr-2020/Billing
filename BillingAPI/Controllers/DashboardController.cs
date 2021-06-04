using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Billing.Services;
using BillingAPI.Filters;
using BillingAPI.Model;
using Core.Model;
using Core.Primitives;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("")]
    [ApiController]
    [AdminAuthorization]
    public class DashboardController : EvarunApiController
    {
        #region CRUD

        /// <summary>
        /// GetUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-users")]
        
        public DataResult<List<UserDto>> GetUsers()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetUsers());
            return result;
        }

        /// <summary>
        /// GetShops
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetShops(s => true));
            return result;
        }

        /// <summary>
        /// GetShop
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-shop")]
        public DataResult<ShopDto> GetShop(int shopid)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetShops(s => s.Id == shopid)?.FirstOrDefault());
            return result;
        }

        /// <summary>
        /// add new shop
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-shop")]
        //[AdminAuthorization]
        public DataResult<ShopDto> AddShop([FromBody] CreateShopModel request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(0, request.Balance, request.Name, request.LifeStyle, request.Owner, request.Specialisations, request.Comment, request.Location));
            return result;
        }

        /// <summary>
        /// edit shop
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-shop")]
        //[AdminAuthorization]
        public DataResult<ShopDto> EditShop([FromBody] CreateShopModel request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(request.ShopId, request.Balance, request.Name, request.LifeStyle, request.Owner, request.Specialisations, request.Comment, request.Location));
            return result;
        }

        /// <summary>
        /// delete shop
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-shop")]
        //[AdminAuthorization]
        public Result DeleteShop(int shopid)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.DeleteShop(shopid));
            return result;
        }

        /// <summary>
        /// GetSpecialisations
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-specialisations")]
        public DataResult<List<SpecialisationDto>> GetSpecialisations()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSpecialisations(s => true));
            return result;
        }

        /// <summary>
        /// Get Specialisation
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-specialisation")]
        public DataResult<SpecialisationDto> GetSpecialisation(int specialisationid)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSpecialisations(s => s.Id == specialisationid).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// add new specialisation
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-specialisation")]
        //[AdminAuthorization]
        public DataResult<SpecialisationDto> AddSpecialisation([FromBody] CreateSpecialisationRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateSpecialisation(0, request.ProductTypeId, request.Name));
            return result;
        }

        /// <summary>
        /// edit specialisation
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-specialisation")]
        //[AdminAuthorization]
        public DataResult<SpecialisationDto> EditSpecialisation([FromBody] CreateSpecialisationRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateSpecialisation(request.SpecialisationId, request.ProductTypeId, request.Name));
            return result;
        }

        /// <summary>
        /// delete specialisation
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-specialisation")]
        //[AdminAuthorization]
        public Result DeleteSpecialisation(int specialisationid)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.DeleteSpecialisation(specialisationid));
            return result;
        }

        /// <summary>
        /// get corporations
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-corporations")]
        public DataResult<List<CorporationDto>> GetCorporations()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetCorporations(r => true));
            return result;
        }

        /// <summary>
        /// get corporation
        /// </summary>
        /// <param name="corporationId"></param>
        /// <returns></returns>
        [HttpGet("a-corporation")]
        public DataResult<CorporationDto> GetCorporation(int corporationId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetCorporations(r => r.Id == corporationId).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// get producttypes
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-producttypes")]
        public DataResult<List<ProductTypeDto>> GetProductTypes()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetProductTypes(p => true));
            return result;
        }

        /// <summary>
        /// get producttype
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        [HttpGet("a-producttype")]
        public DataResult<ProductTypeDto> GetProductType(int productTypeId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetProductTypes(p => p.Id == productTypeId).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// get nomenklaturas
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-nomenklaturas")]
        public DataResult<List<NomenklaturaDto>> GetNomenklaturas()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetNomenklaturas(p => true));
            return result;
        }

        /// <summary>
        /// get nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-nomenklatura")]
        public DataResult<NomenklaturaDto> GetNomenklatura(int nomenklaturaId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetNomenklaturas(p => p.Id == nomenklaturaId).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// get skus
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-skus")]
        public DataResult<List<SkuDto>> GetSkus()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSkus(p => true));
            return result;
        }

        [HttpGet("a-sku")]
        public DataResult<SkuDto> GetSku(int skuId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSkus(p => p.Id == skuId).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// add new nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-nomenklatura")]
        //[AdminAuthorization]
        public DataResult<NomenklaturaDto> AddNomenklatura([FromBody] CreateNomenklaturaRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateNomenklatura(0, request.Name, request.Code, request.SpecialisationId, request.Lifestyle, request.BasePrice, request.BaseCount, request.Description, request.PictureUrl));
            return result;
        }

        /// <summary>
        /// edit nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-nomenklatura")]
        //[AdminAuthorization]
        public DataResult<NomenklaturaDto> EditNomenklatura([FromBody] CreateNomenklaturaRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateNomenklatura(request.Id, request.Name, request.Code, request.SpecialisationId, request.Lifestyle, request.BasePrice, request.BaseCount, request.Description, request.PictureUrl));
            return result;
        }

        /// <summary>
        /// add new nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-sku")]
        //[AdminAuthorization]
        public DataResult<SkuDto> AddSku([FromBody] CreateSkuRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateSku(0, request.NomenklaturaId, request.Count, request.Corporation, request.Name, request.Enabled, request.SkuBasePrice, request.SkuBaseCount));
            return result;
        }

        /// <summary>
        /// edit nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-sku")]
        //[AdminAuthorization]
        public DataResult<SkuDto> EditSku([FromBody] CreateSkuRequest request)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.CreateOrUpdateSku(request.Id, request.NomenklaturaId, request.Count, request.Corporation, request.Name, request.Enabled, request.SkuBasePrice, request.SkuBaseCount));
            return result;
        }

        /// <summary>
        /// delete nomenklatura
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-nomenklatura")]
        //[AdminAuthorization]
        public Result DeleteNomenklatura(int nomenklaturaId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.DeleteNomenklatura(nomenklaturaId));
            return result;
        }

        /// <summary>
        /// delete sku
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-sku")]
        //[AdminAuthorization]
        public Result DeleteSku(int skuId)
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.DeleteSku(skuId));
            return result;
        }

        #endregion

        /// <summary>
        /// get all information about current game session
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-session")]
        public DataResult<SessionDto> GetBillingInfo(int character)
        {
            var service = new AdminService();
            var result = RunAction(() => service.GetSessionInfo(character));
            return result;
        }

        /// <summary>
        /// get Lifestyles
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-lifestyles")]
        public DataResult<List<NamedEntity>> GetLifeStyles()
        {
            var ls = BillingHelper.GetLifestyles();
            var result = RunAction(() => ls);
            return result;
        }

        /// <summary>
        /// get lifestyle
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        [HttpGet("a-lifestyle")]
        public DataResult<NamedEntity> GetLifeStyle(Lifestyles lifestyle)
        {
            var ls = BillingHelper.GetLifestyleDto(lifestyle);
            var result = RunAction(() => ls);
            return result;
        }
    }
}
