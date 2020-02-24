using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public enum TransferType
    {
        Incoming,
        Outcoming
    }

    public class TransferDto
    {
        public string TransferType { get; set; }
        public decimal NewBalance { get; set; }
        public string Comment { get; set; }
        public decimal Amount { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
