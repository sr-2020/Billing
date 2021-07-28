using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Billing.Dto
{
    public class JobLifeStyleDto
    {
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
        public JobLifeStyleDto lastDto { get; set; }
        public decimal K { get; set; }
        public int Insolvent { get; set; }
        public int Irridium { get; set; }
        public int Count { get; set; }

        public decimal GetAvergeKarma()
        {
            if (Count == 0)
                return 0;
            return SumKarma / Count;
        }
        public decimal GetAvergeRents()
        {
            if (Count == 0)
                return 0;
            return SumRents / Count;
        }

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

        public void AddConcurrent(BeatCharacterLocalDto dto)
        {
            lock (this)
            {
                if (dto.IsIrridium)
                {
                    this.Irridium++;
                    return;
                }
                if (dto.Balance < 0)
                {
                    this.Insolvent++;
                    return;
                }
                Count++;
                this.SumKarma += dto.SumKarma;
                this.SumRents += dto.SumRents;
                this.SumDividends += dto.SumDividends;
                this.SumAll += dto.Balance;
                ForecastSumAll += dto.Forecast;
                if (ScoringMin == 0 || dto.Scoringvalue < ScoringMin)
                {
                    ScoringMin = dto.Scoringvalue;
                }
                if (ScoringMax == 0 || dto.Scoringvalue > ScoringMax)
                {
                    ScoringMax = dto.Scoringvalue;
                }
                if (Min == null || ((Min ?? 0) > dto.Balance))
                    Min = dto.Balance;
                if (Max == null || ((Max ?? 0) < dto.Balance))
                    Max = dto.Balance;
                if (ForecastMin == null || ((ForecastMin ?? 0) > dto.Forecast))
                    ForecastMin = dto.Forecast;
                if (ForecastMax == null || ((ForecastMax ?? 0) < dto.Forecast))
                    ForecastMax = dto.Forecast;
            }
        }

    }
}
