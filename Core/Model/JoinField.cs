using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("join_field")]
    public class JoinField : BaseEntity
    {
        [Column("join_id")]
        public int? JoinId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("field_type")]
        public string FieldType { get; set; }
        [Column("game")]
        public int? Game { get; set; }
    }
}
