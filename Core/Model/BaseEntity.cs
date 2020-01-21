using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    public class BaseEntity
    {
        [Column("id")]
        public int Id { get; set; }
    }
}
