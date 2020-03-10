using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("wallet")]
    public class Wallet : BaseEntity
    {
        [Column("balance")]
        public decimal Balance { get; set; }
        [Column("wallet_type")]
        public int WalletType { get; set; }
        [Column("lifestyle")]
        public int Lifestyle { get; set; }
    }
}
