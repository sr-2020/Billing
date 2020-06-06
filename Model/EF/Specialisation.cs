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
        [ForeignKey("product_type")]
        [Column("product_type")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
