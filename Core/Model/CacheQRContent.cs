using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("cache_qr_cont")]
    public class CacheQRContent : BaseEntity
    {
        [Column("qr_id")]
        public long QRID { get; set; }
        [Column("encoded")]
        public string Encoded { get; set; }
    }
}
