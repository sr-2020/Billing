using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("nomenklatura")]
    public class Nomenklatura : BaseEntity
    {
        [Column("code")]
        public string Code { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("base_price")]
        public decimal BasePrice { get; set; }
        [Column("lifestyle")]
        public int Lifestyle { get; set; }
        [ForeignKey("product_type")]
        [Column("product_type")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        [Column("picture_url")]
        public string PictureUrl { get; set; }
    }
}
