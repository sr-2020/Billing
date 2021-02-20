using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("specialisation")]
    public class Specialisation : BaseEntity
    {
        [ForeignKey("product_type")]
        [Column("product_type")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public string Name { get; set; }
    }
}
