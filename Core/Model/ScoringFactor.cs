using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring_factor")]
    public class ScoringFactor : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
        [ForeignKey("scoring_category")]
        [Column("scoring_category")]
        public int CategoryId { get; set; }
        public virtual ScoringCategory Category { get; set; }
        public string Code { get; set; }
    }
}
