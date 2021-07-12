using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class BeatCharacterLocalDto
    {
        public decimal SumDividends { get; set; }
        public decimal SumKarma { get; set; }
        public decimal SumRents { get; set; }
        public decimal Scoringvalue { get; set; }
        public bool IsIrridium { get; set; }
        public decimal Balance { get; set; }
        public decimal Forecast { get; set; }
    }
}
