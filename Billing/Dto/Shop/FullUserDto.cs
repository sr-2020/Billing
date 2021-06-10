using Billing.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class FullUserDto
    {
        public BalanceDto Sin { get; set; }
        public RentaSumDto Rents { get; set; }
        public TransferSum Transfers { get; set; }
        public ScoringDto Scoring { get; set; }
        public bool IsAdmin { get; set; }
    }
}
