using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("system_settings")]
    public class SystemSettings: BaseEntity
    {
        [Column("key")]
        public string Key { get; set; }
        [Column("value")]
        public string Value { get; set; }
    }
}
