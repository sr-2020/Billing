using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("contract_limit")]
    public class ContractLimit : BaseEntity
    {
        [Column("contract")]
        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; }
        [Column("sku")]
        public int SkuId { get; set; }
        public virtual Sku Sku { get; set; }
    }
}
