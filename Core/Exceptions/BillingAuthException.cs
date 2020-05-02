using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class BillingAuthException : BillingException
    {
        public BillingAuthException() : base() { }
        public BillingAuthException(string message) : base(message) { }
        public BillingAuthException(string message, Exception innerException) : base(message, innerException) { }
    }
}
