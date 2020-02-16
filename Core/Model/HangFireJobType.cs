using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("hangfire_job_type")]
    public class HangFireJobType : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
    }
}
