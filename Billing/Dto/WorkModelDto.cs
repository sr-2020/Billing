using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class WorkModelDto
    {
        public bool Dividends1 { get; set; }
        public bool Dividends2 { get; set; }
        public bool Dividends3 { get; set; }
        public decimal StockGainPercentage { get; set; }
        public decimal KarmaCount { get; set; }
    }
}
