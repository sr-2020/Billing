using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class InsolventDto
    {
        public string Citizenship { get; set; }
        public string SinText { get; set; }
        public string PersonName { get; set; }
        public int ModelId { get; set; }
        public decimal SumRents { get; set; }
        public decimal SumOverdraft { get; set; }
        public decimal Balance { get; set; }
        public List<Debt> Debts { get; set; }
    }

    public class Debt
    {
        public string SkuName { get; set; }
        public int RentId { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
