using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_action")]
    public class BillingAction : BaseEntity
    {
        [Column("alias")]
        public string Alias { get; set; }
        [Column("cycle")]
        public int Cycle { get; set; }
        [Column("beat")]
        public int Beat { get; set; }
        [Column("enabled")]
        public bool Enabled { get; set; }
    }
}
