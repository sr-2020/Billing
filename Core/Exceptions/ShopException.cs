using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class ShopException : Exception
    {
        public ShopException() : base() { }
        public ShopException(string message) : base(message) { }
        public ShopException(string message, Exception innerException) : base(message, innerException) { }
    }
}
