using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto.Shop;
using Billing.DTO;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class DashboardController : EvarunApiController
    {

        /// <summary>
        /// GetShops
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetShops(s => true));
            return result;
        }

        /// <summary>
        /// GetShops
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-shop")]
        public DataResult<ShopDto> GetShop(int shopid)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetShops(s => s.Id == shopid)?.FirstOrDefault());
            return result;
        }

        /// <summary>
        /// add new shop
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-shop")]
        public DataResult<ShopDto> AddShop([FromBody] CreateShopModel request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(0, request.Amount, request.Name, request.LifeStyle, request.Owner));
            return result;
        }

        /// <summary>
        /// edit shop
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-shop")]
        public DataResult<ShopDto> EditShop([FromBody] CreateShopModel request)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var result = RunAction(() => manager.CreateOrUpdateShopWallet(request.ShopId, request.Amount, request.Name, request.LifeStyle, request.Owner));
            return result;
        }

        /// <summary>
        /// delete shop
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-shop")]
        public Result DeleteShop(int shopid)
        {
            var manager = IocContainer.Get<IBillingManager>();
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
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetSpecialisations(s => true));
            return result;
        }

        /// <summary>
        /// Get Specialisation
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-specialisation")]
        public DataResult<SpecialisationDto> GetSpecialisation(int shopid)
        {
            var manager = IocContainer.Get<IShopManager>();
            var result = RunAction(() => manager.GetSpecialisations(s => s.Id == shopid).FirstOrDefault());
            return result;
        }

        /// <summary>
        /// add new specialisation
        /// </summary>
        /// <returns></returns>
        [HttpPost("a-add-specialisation")]
        public DataResult<ShopDto> AddSpecialisation([FromBody] CreateSpecialisationRequest request)
        {
            throw new NotImplementedException();
            //var manager = IocContainer.Get<IBillingManager>();
            //var result = RunAction(() => manager.CreateOrUpdateShopWallet(0, request.Amount, request.Name, request.LifeStyle, request.Owner));
            //return result;
        }

        /// <summary>
        /// edit specialisation
        /// </summary>
        /// <returns></returns>
        [HttpPatch("a-edit-specialisation")]
        public DataResult<ShopDto> EditSpecialisation([FromBody] CreateSpecialisationRequest request)
        {
            throw new NotImplementedException();
            //var manager = IocContainer.Get<IBillingManager>();
            //var result = RunAction(() => manager.CreateOrUpdateShopWallet(request.ShopId, request.Amount, request.Name, request.LifeStyle, request.Owner));
            //return result;
        }

        /// <summary>
        /// delete specialisation
        /// </summary>
        /// <returns></returns>
        [HttpDelete("a-del-specialisation")]
        public Result DeleteSpecialisation(int shopid)
        {
            throw new NotImplementedException();
            //var manager = IocContainer.Get<IBillingManager>();
            //var result = RunAction(() => manager.DeleteShop(shopid));
            //return result;
        }
    }
}
