using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class SinInfo
    {
        public int SIN { get; set; }
        public decimal Balance { get; set; }
        //public SinLifeStyle CurrentLifeStyle { get; set; }
    }
}
