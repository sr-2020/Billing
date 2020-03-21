using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("contract")]
    public class Contract : BaseEntity
    {
        [ForeignKey("shop")]
        [Column("shop")]
        public int ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        [ForeignKey("corporation")]
        [Column("corporation")]
        public int CorporationId { get; set; }
        public virtual CorporationWallet Corporation { get; set; }

    }
}
