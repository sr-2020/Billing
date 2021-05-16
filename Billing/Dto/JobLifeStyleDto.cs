using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class JobLifeStyleDto
    {
        public decimal SumAll { get; set; }
        public decimal ForecastSumAll { get; set; }
        public decimal? Min { get; set; }
        public decimal? ForecastMin { get; set; }
        public decimal? Max { get; set; }
        public decimal? ForecastMax { get; set; }
        public int Count { get; set; }

        public decimal Bronze()
        {
            return (Min ?? 0) + 1 / 3 * (SumAll / Count - (Min ?? 0));
        }

        public decimal ForecastBronze()
        {
            return (ForecastMin ?? 0) + 1 / 3 * (ForecastSumAll / Count - (ForecastMin ?? 0));
        }

        public decimal Silver()
        {
            return (Min ?? 0) + 2 / 3 * (SumAll / Count - (Min ?? 0));
        }

        public decimal ForecastSilver()
        {
            return (ForecastMin ?? 0) + 2 / 3 * (ForecastSumAll / Count - (ForecastMin ?? 0));
        }

        public decimal Gold()
        {
            return SumAll / Count + 1/3 * ((Max ?? 0) - SumAll / Count);
        }

        public decimal ForecastGold()
        {
            return ForecastSumAll / Count + 1 / 3 * ((ForecastMax ?? 0) - ForecastSumAll / Count);
        }

        public decimal Platinum()
        {
            return SumAll / Count + 2 / 3 * ((Max ?? 0) - SumAll / Count);
        }

        public decimal ForecastPlatinum()
        {
            return ForecastSumAll / Count + 2 / 3 * ((ForecastMax ?? 0) - ForecastSumAll / Count);
        }

    }
}
