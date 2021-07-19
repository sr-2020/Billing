using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing.Dto;
using Billing.Dto.Scoring;
using BillingAPI.Model;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scoringspace;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoringController : EvarunApiController
    {

        [HttpGet("info/getmyscoring")]
        public DataResult<ScoringDto> GetScoring(int character)
        {
            var manager = IocContainer.Get<IScoringManager>();
            return RunAction(()=> manager.GetFullScoring(character), $"get full scoring {character}");
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