using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sin")]
    public class SIN : BaseEntity
    {
        [Column("sin_text")]
        public string Sin { get; set; }
        [Column("person_name")]
        public string PersonName { get; set; }
        [Column("race")]
        public int Race { get; set; }
        [Column("citizenship")]
        public int Citizenship { get; set; }
        [Column("character")]
        [ForeignKey("character")]
        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }

    }
}
