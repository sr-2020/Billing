using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("price")]
    public class Price : BaseEntity
    {
        [ForeignKey("sku")]
        [Column("sku")]
        public int SkuId { get; set; }
        public virtual Sku Sku { get; set; }
        [ForeignKey("shop")]
        [Column("shop")]
        public int ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        [Column("character")]
        public int CharacterId { get; set; }
        [Column("base_price")]
        public decimal BasePrice { get; set; }
        [Column("date_created")]
        public DateTime DateCreated { get; set; }
        [Column("scoring")]
        public decimal CurrentScoring { get; set; }
        [Column("discount")]
        public decimal Discount { get; set; }
        [Column("shop_comission")]
        public decimal ShopComission { get; set; }
        [Column("final_price")]
        public decimal FinalPrice { get; set; }
    }
}
