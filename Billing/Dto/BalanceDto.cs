using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class BalanceDto
    {
        public int CharacterId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CurrentScoring { get; set; }
        public string LifeStyle { get; set; }
        public decimal ForecastLifeStyle { get; set; }
    }
}
