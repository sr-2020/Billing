﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class BalanceDto
    {
        public int CharacterId { get; set; }
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
    }
}
