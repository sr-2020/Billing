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
        [Column("forecast_balance")]
        public decimal IncomeOutcome { get; set; }
        [Column("wallet_type")]
        public int WalletType { get; set; }
        [Column("current_lifestyle")]
        public int? LifeStyle { get; set; }
        [Column("is_irridium")]
        public bool IsIrridium { get; set; }
    }
}
