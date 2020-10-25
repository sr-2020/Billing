using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("metatype")]
    public class Metatype: BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("alias")]
        public string Alias { get; set; }
        [Column("cost")]
        public int Cost { get; set; }
        [Column("hidden")]
        public bool Hidden { get; set; }
    }
}
