using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class BillingException : Exception
    {
        public BillingException() : base() { }
        public BillingException(string message) : base(message) { }
        public BillingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
