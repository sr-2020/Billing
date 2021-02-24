using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    public class OwnerEntity : NamedEntity
    {
        [Column("owner")]
        [ForeignKey("owner")]
        public int? OwnerId { get; set; }
        public Character Owner { get; set; }
    }
}
