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
        [Column("last_kpi")]
        public decimal LastKPI { get; set; }
        [Column("current_kpi")]
        public decimal CurrentKPI { get; set; }
        [Column("sku_sold")]
        public decimal SkuSold { get; set; }
        [Column("sku_sold_last")]
        public decimal LastSkuSold { get; set; }
        public virtual List<CorporationSpecialisation> Specialisations { get; set; }
    }
}
