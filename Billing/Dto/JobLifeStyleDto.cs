using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class JobLifeStyleDto
    {
        public decimal ScoringComposition { get; set; }
        public decimal ScoringMin { get; set; }
        public decimal ScoringMax { get; set; }
        public decimal SumAll { get; set; }
        public decimal ForecastSumAll { get; set; }
        public decimal? Min { get; set; }
        public decimal? ForecastMin { get; set; }
        public decimal? Max { get; set; }
        public decimal? ForecastMax { get; set; }
        public decimal SumRents { get; set; }
        public decimal SumKarma { get; set; }
        public decimal SumDividends { get; set; }
        public int Insolvent { get; set; }
        public int Irridium { get; set; }
        public int Count { get; set; } 

        public decimal Bronze()
        {
            return (Min ?? 0) + (SumAll / Count - (Min ?? 0)) / 3;
        }

        public decimal ForecastBronze()
        {
            return (ForecastMin ?? 0) + (ForecastSumAll / Count - (ForecastMin ?? 0)) / 3;
        }

        public decimal Silver()
        {
            return (Min ?? 0) + 2 * (SumAll / Count - (Min ?? 0)) / 3;
        }

        public decimal ForecastSilver()
        {
            return (ForecastMin ?? 0) + 2 * (ForecastSumAll / Count - (ForecastMin ?? 0)) / 3;
        }

        public decimal Gold()
        {
            return SumAll / Count + ((Max ?? 0) - SumAll / Count) / 3;
        }

        public decimal ForecastGold()
        {
            return ForecastSumAll / Count + ((ForecastMax ?? 0) - ForecastSumAll / Count) / 3;
        }

        public decimal Platinum()
        {
            return SumAll / Count + 2 * ((Max ?? 0) - SumAll / Count) / 3;
        }

        public decimal ForecastPlatinum()
        {
            return ForecastSumAll / Count + 2 * ((ForecastMax ?? 0) - ForecastSumAll / Count) / 3;
        }

    }
}
