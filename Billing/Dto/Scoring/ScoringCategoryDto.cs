using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
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
        public decimal Value { get; set; }
        public decimal Weight { get; set; }
        public List<ScoringFactorDto> Factors { get; set; }
    }
}
