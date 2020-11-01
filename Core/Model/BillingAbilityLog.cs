using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_ability_used")]
    public class BillingAbilityLog:BaseEntity
    {
        [Column("message")]
        public string Message { get; set; }
        [Column("key")]
        public string Key { get; set; }
    }
}
