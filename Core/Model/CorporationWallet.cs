using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("corporation_wallet")]
    public class CorporationWallet : OwnerEntity
    {
        [ForeignKey("wallet")]
        [Column("wallet")]
        public int? WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [Column("logo_url")]
        public string CorporationLogoUrl { get; set; }
        [Column("alias")]
        public string Alias { get; set; }
    }
}
