using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class WalletDto
    {
        public int WalletId { get; set; }
        public WalletTypes WalletType { get; set; }
    }
}
