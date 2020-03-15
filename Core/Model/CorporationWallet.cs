using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("corporation_wallet")]
    public class CorporationWallet : BaseEntity
    {
        [Column("id_foreign")]
        public int Foreign { get; set; }
        [ForeignKey("wallet")]
        [Column("wallet")]
        public int? WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
