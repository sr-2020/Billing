using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    public class NamedEntity: BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
    }
}
