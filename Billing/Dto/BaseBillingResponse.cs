using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public enum TransferCode
    {
        Ok = 0,
        Nomoney = 1,
        Locked = 2,
        Error = 3
    }

    public class BaseBillingResponse
    {
        public TransferCode Status { get; set; }
        public string Message { get; set; }
    }
}
