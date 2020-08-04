using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("join_character")]
    public class JoinCharacter : BaseEntity
    {
        [ForeignKey("character")]
        [Column("character")]
        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }
        [Column("name")]
        public string Name { get; set; }

    }
}
