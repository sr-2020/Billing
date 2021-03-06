using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_cycle")]
    public class BillingCycle : BaseEntity
    {
        [Column("number")]
        public int Number { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("token")]
        public string Token { get; set; }
    }
}
