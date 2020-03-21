using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("specialisation")]
    public class Specialisation : BaseEntity
    {
        [ForeignKey("shop")]
        [Column("shop")]
        public int ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        [ForeignKey("nomenklatura")]
        [Column("nomenklatura")]
        public int NomenklaturaId { get; set; }
        public virtual Nomenklatura Nomenklatura { get; set; }
    }
}
