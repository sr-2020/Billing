using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("qr_shop")]
    public class ShopQR : BaseEntity
    {
        [ForeignKey("shop")]
        [Column("shop")]
        public int? ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        [ForeignKey("sku")]
        [Column("sku")]
        public int? SkuId { get; set; }
        public virtual Sku Sku { get; set; }
    }
}
