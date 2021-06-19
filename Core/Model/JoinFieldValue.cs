using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("join_field_value")]
    public class JoinFieldValue : BaseEntity
    {
        [Column("value")]
        public string Value { get; set; }
        [ForeignKey("join_character")]
        [Column("join_character")]
        public int JoinCharacterId { get; set; }
        public JoinCharacter JoinCharacter { get; set; }
        [ForeignKey("join_field")]
        [Column("join_field")]
        public int JoinFieldId { get; set; }
        public JoinField JoinField { get; set; }
    }
}
