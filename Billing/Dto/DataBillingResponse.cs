using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class DataBillingResponse<T> : BaseBillingResponse
    {
        public T Data { get; set; }
    }
}
