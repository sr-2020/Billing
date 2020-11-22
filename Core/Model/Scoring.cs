using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("scoring")]
    public class Scoring : BaseEntity
    {
        [Column("current_scoring_fix")]
        public decimal CurrentFix { get; set; }
        [Column("current_scoring_relative")]
        public decimal CurerentRelative { get; set; }
    }
}
