using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class SuggestContractRequest : CorporationBasedRequest
    {
        public int Shop { get; set; }
    }

    public class RequestTerminateContractRequest : CorporationBasedRequest
    {
        public int Shop { get; set; }
    }
    public class ApproveContractRequest : ShopBasedRequest
    {
        public int Corporation { get; set; }
    }
    public class TerminateContractRequest : ShopBasedRequest
    {
        public int Corporation { get; set; }
    }

}
