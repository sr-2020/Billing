using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{ 
    public class BillingUnauthorizedException : BillingException
    {
        public BillingUnauthorizedException() : base() { }
        public BillingUnauthorizedException(string message) : base(message) { }
        public BillingUnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
