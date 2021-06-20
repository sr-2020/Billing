using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("hack_history")]
    public class HackerHistory : BaseEntity
    {
        [Column("action")]
        public string Action { get; set; }
        [Column("main")]
        public int? Main { get; set; }
        [Column("secondid")]
        public int? Second { get; set; }
        [Column("parameters")]
        public string Parameters { get; set; }
    }
}
