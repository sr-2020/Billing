using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("transfer")]
    public class Transfer : BaseEntity
    {
        [Column("wallet_from")]
        [ForeignKey("WalletFrom")]
        public int WalletFromId { get; set; }
        public virtual Wallet WalletFrom { get; set; }
        [Column("wallet_to")]
        [ForeignKey("wallet_to")]
        public int WalletToId { get; set; }
        public virtual Wallet WalletTo { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
        [Column("renta")]
        public int? Renta { get; set; }
        [Column("newlifestyle_from")]
        public int NewLifeStyleFrom { get; set; }
        [Column("newlifestyle_to")]
        public int NewLifeStyleTo { get; set; }
    }
}
