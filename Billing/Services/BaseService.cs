using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Services
{
    public class BaseService
    {
        public BaseService()
        {
            Factory = new ManagerFactory();
        }
        protected ManagerFactory Factory { get; set; }
    }
}
