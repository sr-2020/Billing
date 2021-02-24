using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("nomenklatura")]
    public class Nomenklatura : NamedEntity
    {
        [Column("code")]
        public string Code { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("base_price")]
        public decimal BasePrice { get; set; }
        [Column("base_count")]
        public int BaseCount { get; set; }
        [Column("lifestyle")]
        public int Lifestyle { get; set; }
        [ForeignKey("specialisation")]
        [Column("specialisation")]
        public int SpecialisationId { get; set; }
        public virtual Specialisation Specialisation { get; set; }
        [Column("picture_url")]
        public string PictureUrl { get; set; }
        [Column("external_id")]
        public int ExternalId { get; set; }
        [Column("secret")]
        public string Secret { get; set; }
    }
}
