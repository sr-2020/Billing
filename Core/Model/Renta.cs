using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("renta")]
    public class Renta : BaseEntity
    {
        [ForeignKey("sku")]
        [Column("sku")]
        public int SkuId { get; set; }
        public virtual Sku Sku { get; set; }
        [ForeignKey("shop")]
        [Column("shop")]
        public int ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        //[Column("character")]
        //public int CharacterId { get; set; }
        [ForeignKey("sin")]
        [Column("sin")]
        public int SinId { get; set; }
        public SIN Sin { get; set; }
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
        [Column("has_qr_write")]
        public bool HasQRWrite { get; set; }
        [Column("qr_recorded")]
        public string QRRecorded { get; set; }
        [ForeignKey("price")]
        [Column("price")]
        public int PriceId { get; set; }
        public virtual Price Price { get; set; }
        [Column("secret")]
        public string Secret { get; set; }
        [Column("lifestyle")]
        public int LifeStyle { get; set; }
    }
}
