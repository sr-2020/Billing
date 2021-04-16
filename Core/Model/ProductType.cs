using Core.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("product_type")]
    public class ProductType : NamedEntity
    {
        [Column("discount_type")]
        public int DiscountType { get; set; }
        [Column("external_id")]
        public int ExternalId { get; set; }
        [Column("alias")]
        public string Alias { get; set; }

        public bool InstantConsume { get => this.Alias == ProductTypeEnum.EdibleFood.ToString(); }
    }
}
