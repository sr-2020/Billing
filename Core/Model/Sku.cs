using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sku")]
    public class Sku : BaseEntity
    {
        [Column("nomenklatura")]
        public virtual Nomenklatura Nomenklatura { get; set; }
        [Column("count")]
        public int Count { get; set; }
        [ForeignKey("corporation")]
        [Column("corporation")]
        public int CorporationId { get; set; }
        public virtual CorporationWallet Corporation { get; set; }
        [Column("enabled")]
        public bool Enabled { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
