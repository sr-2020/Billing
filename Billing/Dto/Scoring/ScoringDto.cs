using Billing.Dto.Scoring;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class ScoringDto
    {
        public int Character { get; set; }
        public decimal CurrentFix { get; set; }
        public decimal CurrentRelative { get; set; }
        public List<CurrentScoringCategoryDto> RelativeCategories {get;set;}
        public List<CurrentScoringCategoryDto> FixCategories { get; set; }
    }
}
