using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    [Obsolete]
    public class BalanceDtoOld
    {
        public int CharacterId { get; set; }
        public int ModelId { get; set; }
        public string SIN { get; set; }
        public decimal CurrentBalance { get; set; }
        public string PersonName { get; set; }
        public decimal CurrentScoring { get; set; }
        public string LifeStyle { get; set; }
        public string ForecastLifeStyle { get; set; }
        public string Metatype { get; set; }
        public string Citizenship { get; set; }
        public string Nationality { get; set; }
        public string Status { get; set; }
        public string Nation { get; set; }
        public string Viza { get; set; }
        public string Pledgee { get; set; }
    }

    public class BalanceDto
    {
        public int ModelId { get; set; }
        public string SIN { get; set; }
        public decimal CurrentBalance { get; set; }
        public string PersonName { get; set; }
        public decimal CurrentScoring { get; set; }
        public string LifeStyle { get; set; }
        public string ForecastLifeStyle { get; set; }
        public string Metatype { get; set; }
        public string Citizenship { get; set; }
        public string Nationality { get; set; }
        public string Status { get; set; }
        public string Nation { get; set; }
        public string Viza { get; set; }
        public string Pledgee { get; set; }
    }
}
