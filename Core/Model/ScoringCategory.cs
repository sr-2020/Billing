using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring_category")]
    public class ScoringCategory : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("weight")]
        public decimal Weight { get; set; }
    }
}
