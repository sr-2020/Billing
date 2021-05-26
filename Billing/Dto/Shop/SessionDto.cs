using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class SessionDto
    {
        public CycleDto Cycle { get; set; }
        public LifeStyleDto LifeStyle { get; set; }
        public string Deploy { get; set; }
        public class CycleDto 
        { 
            public string Token { get; set; }
            public int Number { get; set; }
            public bool IsActive { get; set; }
            public CycleDto(BillingCycle cycle)
            {
                Token = cycle.Token;
                Number = cycle.Number;
                IsActive = cycle.IsActive;
            }
        }

        public BeatCharactersDto BeatCharacters { get; set; }
        public class BeatCharactersDto
        {
            public int Number { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
            public decimal SumAll { get; set; }
            public decimal ForecastSumAll { get; set; }
            public decimal? Min { get; set; }
            public decimal? ForecastMin { get; set; }
            public decimal? Max { get; set; }
            public decimal? ForecastMax { get; set; }
            public decimal SumRents { get; set; }
            public decimal SumKarma { get; set; }
            public int Insolvent { get; set; }
            public int Irridium { get; set; }
            public int Count { get; set; }
            public BeatCharactersDto(BillingBeat beat, JobLifeStyleDto ls)
            {
                if (beat == null || ls == null)
                    return;

                Number = beat.Number;
                StartTime = beat.StartTime;
                FinishTime = beat.FinishTime;
                SumAll = ls.SumAll;
                ForecastSumAll = ls.ForecastSumAll;
                Min = ls.Min;
                ForecastMin = ls.ForecastMin;
                Max = ls.Max;
                ForecastMax = ls.ForecastMax;
                SumRents = ls.SumRents;
                SumKarma = ls.SumKarma;
                Insolvent = ls.Insolvent;
                Irridium = ls.Irridium;
                Count = ls.Count;
            }
        }

        public class LifeStyleDto
        {
            public decimal Bronze { get; set; }
            public decimal Silver { get; set; }
            public decimal Gold { get; set; }
            public decimal Platinum { get; set; }
            public decimal ForecastBronze { get; set; }
            public decimal ForecastSilver { get; set; }
            public decimal ForecastGold { get; set; }
            public decimal ForecastPlatinum { get; set; }
            public LifeStyleDto(LifeStyleAppDto appdto)
            {
                Bronze = appdto.Bronze;
                Silver = appdto.Silver;
                Gold = appdto.Gold;
                Platinum = appdto.Platinum;
                ForecastBronze = appdto.ForecastBronze;
                ForecastSilver = appdto.ForecastSilver;
                ForecastGold = appdto.ForecastGold;
                ForecastPlatinum = appdto.ForecastPlatinum;
            }
        }
    }
}
