using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Scoring
{
    public class ScoringFactorDto
    {
        public ScoringFactorDto(ScoringFactor factor)
        {
            Id = factor.Id;
            Name = factor.Name;
            CategoryId = factor.Category?.Id;
            CategoryName = factor.Category?.Name;
            CategoryType = factor.Category?.CategoryType;
            CategoryWeight = factor.Category?.Weight;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryType { get; set; }
        public decimal? CategoryWeight { get; set; }
    }
}
