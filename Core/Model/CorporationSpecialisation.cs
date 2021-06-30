using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("corporation_specialisation")]
    public class CorporationSpecialisation : BaseEntity
    {
        [ForeignKey("corporation")]
        [Column("corporation")]
        public int CorporationId { get; set; }
        public CorporationWallet Corporation { get; set; }
        [ForeignKey("specialisation")]
        [Column("specialisation")]
        public int SpecialisationId { get; set; }
        public Specialisation Specialisation { get; set; }
        [Column("ratio")]
        public decimal Ratio { get; set; }
    }
}
