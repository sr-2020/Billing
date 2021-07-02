using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
    public class CurrentScoringCategoryDto : ScoringCategoryDto
    {
        public CurrentScoringCategoryDto(CurrentCategory currentCategory, decimal currentSum, decimal currentResult) : base(currentCategory.Category)
        {
            var k = currentResult / (currentSum != 0 ? currentSum : 1);
            Value = Math.Round((currentCategory?.Value ?? 0) * k, 2) ;
        }
        public decimal Value { get; set; }
    }

    public class ScoringCategoryDto
    {
        public ScoringCategoryDto(ScoringCategory category)
        {
            Id = category.Id;
            Name = category.Name;
            Weight = category.Weight;
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal Weight { get; set; }
        public List<ScoringFactorDto> Factors { get; set; }
    }
}
