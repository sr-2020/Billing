using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Transfer : BaseEntity
    {
        public Wallet WalletFrom { get; set; }
        public Wallet WalletTo { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public Credit Dogovor { get; set; }
    }
}
