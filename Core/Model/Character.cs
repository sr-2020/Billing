using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("character")]
    public class Character : BaseEntity
    {
        [Column("model_id")]
        public int Model { get; set; } //main!!! Its real Character

    }
}
