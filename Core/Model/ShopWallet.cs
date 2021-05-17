using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("shop_wallet")]
    public class ShopWallet : OwnerEntity
    {
        [ForeignKey("wallet")]
        [Column("wallet")]
        public int? WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [Column("lifestyle")]
        public int LifeStyle { get; set; }
        [Column("commission")]
        public decimal Commission { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
        [Column("master")]
        public string Location { get; set; }
        public virtual List<ShopSpecialisation> Specialisations { get; set; }
    }
}
