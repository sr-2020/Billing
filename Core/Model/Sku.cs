using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sku")]
    public class Sku : NamedEntity
    {
        [Column("nomenklatura")]
        public int NomenklaturaId { get; set; }
        public virtual Nomenklatura Nomenklatura { get; set; }
        [Column("count")]
        public int Count { get; set; }
        [ForeignKey("corporation")]
        [Column("corporation")]
        public int CorporationId { get; set; }
        public virtual CorporationWallet Corporation { get; set; }
        [Column("enabled")]
        public bool Enabled { get; set; }
        [Column("external_id")]
        public int ExternalId { get; set; }
        [Column("sku_base_price")]
        public int? SkuBasePrice { get; set; }
        [Column("sku_base_count")]
        public int? SkuBaseCount { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
    }
}
