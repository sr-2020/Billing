using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class BillingNotFoundException : BillingException
    {
        public BillingNotFoundException() : base() { }
        public BillingNotFoundException(string message) : base(message) { }
        public BillingNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
