using Billing;
using Billing.Dto;
using Billing.Dto.Scoring;
using Billing.Dto.Shop;
using Billing.DTO;
using Billing.Services;
using BillingAPI.Filters;
using BillingAPI.Model;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Scoringspace;
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
        /// GetUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
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
        [HttpGet("shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetShops(s => true));
            return result;
        }

        /// <summary>
        /// GetSpecialisations
        /// </summary>
        /// <returns></returns>
        [HttpGet("specialisations")]
        public DataResult<List<SpecialisationDto>> GetSpecialisations()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSpecialisations(s => true));
            return result;
        }

        /// <summary>
        /// get corporations
        /// </summary>
        /// <returns></returns>
        [HttpGet("corporations")]
        public DataResult<List<CorporationDto>> GetCorporations()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetCorporationDtos(r => true));
            return result;
        }

        /// <summary>
        /// get producttypes
        /// </summary>
        /// <returns></returns>
        [HttpGet("producttypes")]
        public DataResult<List<ProductTypeDto>> GetProductTypes()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetProductTypes(p => true));
            return result;
        }

        /// <summary>
        /// get nomenklaturas
        /// </summary>
        /// <returns></returns>
        [HttpGet("nomenklaturas")]
        public DataResult<List<NomenklaturaDto>> GetNomenklaturas()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetNomenklaturas(p => true));
            return result;
        }

        /// <summary>
        /// get skus
        /// </summary>
        /// <returns></returns>
        [HttpGet("skus")]
        public DataResult<List<SkuDto>> GetSkus()
        {
            var manager = IocContainer.Get<IAdminManager>();
            var result = RunAction(() => manager.GetSkus(p => true));
            return result;
        }
        /// <summary>
        /// get Lifestyles
        /// </summary>
        /// <returns></returns>
        [HttpGet("lifestyles")]
        public DataResult<List<NamedEntity>> GetLifeStyles()
        {
            var ls = BillingHelper.GetLifestyles();
            var result = RunAction(() => ls);
            return result;
        }

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

        /// <summary>
        /// GetUser 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        public DataResult<FullUserDto> GetUser(int character)
        {
            var service = new AdminService();
            var result = RunAction(() => service.GetFullUser(character));
            return result;
        }
        /// <summary>
        /// AddScoringCategory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("a-add-scoringcategory")]
        public DataResult<ScoringCategoryDto> AddScoringCategory([FromBody] AddScoringCategoryRequest request)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.AddScoringCategory(request.CategoryName, request.Relative, request.Weight));
        }

        /// <summary>
        /// AddScoringEvent
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("a-add-scoringevent")]
        public DataResult<ScoringEventLifeStyleDto> AddScoringEvent([FromBody] AddScoringEventRequest request)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.AddScoringEvent(request.FactorId, request.Lifestyle, request.Value));
        }

        /// <summary>
        /// GetScoringCategories
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-scoringcategories")]
        public DataResult<List<ScoringCategoryDto>> GetScoringCategories(bool? relative)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.GetScoringCategories(relative));
        }

        /// <summary>
        /// GetScoringFactors
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-scoringfactors")]
        public DataResult<List<ScoringFactorDto>> GetScoringFactors(int categoryId)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.GetScoringFactors(categoryId));
        }

        /// <summary>
        /// GetFactorEvents
        /// </summary>
        /// <returns></returns>
        [HttpGet("a-scoringevents")]
        public DataResult<List<ScoringEventLifeStyleDto>> GetFactorEvents(int factorId)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.GetFactorEvents(factorId));
        }

        /// <summary>
        /// UpdateCategoryWeight
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("a-edit-scoringcategory")]
        public DataResult<ScoringCategoryDto> UpdateCategoryWeight([FromBody] UpdateCategoryWeightRequest request)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.UpdateCategoryWeight(request.CategoryId, request.Weight));
        }

        /// <summary>
        /// UpdateFactorCategory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("a-edit-factorcategory")]
        public DataResult<ScoringFactorDto> UpdateFactorCategory([FromBody] UpdateFactorCategoryRequest request)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.UpdateFactorCategory(request.FactorId, request.NewCategoryId));
        }

        /// <summary>
        /// DeleteScoringCategory
        /// </summary>
        [HttpDelete("a-del-scoringcategory")]
        public Result DeleteScoringCategory(int id)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.DeleteScoringCategory(id));
        }

        /// <summary>
        /// DeleleteScoringEvent
        /// </summary>
        [HttpDelete("a-del-scoringevent")]
        public Result DeleleteScoringEvent(int factorId, int lifestyle)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(() => manager.DeleleteScoringEvent(factorId, lifestyle));
        }
    }
}
