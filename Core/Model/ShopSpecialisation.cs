using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("shop_specialisation")]
    public class ShopSpecialisation : BaseEntity
    {
        [ForeignKey("shop")]
        [Column("shop")]
        public int ShopId { get; set; }
        public virtual ShopWallet Shop { get; set; }
        [ForeignKey("specialisation")]
        [Column("specialisation")]
        public int SpecialisationId { get; set; }
        public virtual Specialisation Specialisation { get; set; }
    }
}
