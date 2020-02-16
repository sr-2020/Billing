using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sin_details")]
    public class SINDetails : BaseEntity
    {
        [ForeignKey("wallet")]
        [Column("wallet")]
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [Column("sin")]
        [ForeignKey("sin")]
        public int SINId { get; set; }
        public virtual SIN SIN { get; set; }
        [Column("scoring")]
        public int? Scoring { get; set; }
        [Column("work")]
        public int? Work { get; set; }
        [Column("ikar")]
        public int? IKAR { get; set; }
    }
}
