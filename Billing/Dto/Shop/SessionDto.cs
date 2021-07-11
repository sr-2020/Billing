using Billing.DTO;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing.Dto.Shop
{
    public class SessionDto
    {
        public CycleDto Cycle { get; set; }
        public LifeStyleDto LifeStyle { get; set; }
        public string Deploy { get; set; }
        public string PersonName { get; set; }
        public BeatCharactersDto BeatCharacters { get; set; }
        public BeatItemsDto BeatItems { get; set; }

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

        public class BeatCharactersDto : BeatDto
        {
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
            public BeatCharactersDto(BillingBeat beat, JobLifeStyleDto ls)
                : base(beat)
            {
                if (beat == null || ls == null)
                    return;
                SumAll = ls.SumAll;
                ForecastSumAll = ls.ForecastSumAll;
                Min = ls.Min;
                ForecastMin = ls.ForecastMin;
                Max = ls.Max;
                ForecastMax = ls.ForecastMax;
                SumRents = ls.SumRents;
                SumKarma = ls.SumKarma;
                SumDividends = ls.SumDividends;
                Insolvent = ls.Insolvent;
                Irridium = ls.Irridium;
                Count = ls.Count;
            }
        }

        public class BeatItemsDto : BeatDto
        {
            public BeatItemsDto(BillingBeat beat, List<CorporationDto> corporations, List<ShopDto> shops)
                : base(beat)
            {
                if (beat == null || corporations == null)
                    return;
                Corporations = corporations;
                ShopsSum = shops.Sum(s => s.Balance);
            }
            public List<CorporationDto> Corporations { get; set; }
            public decimal ShopsSum { get; set; }

        }

        public class BeatDto
        {
            public BeatDto(BillingBeat beat)
            {
                if (beat == null)
                    return;
                Number = beat.Number;
                StartTime = beat.StartTime;
                FinishTime = beat.FinishTime;
            }
            public int Number { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
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
                Bronze = BillingHelper.Round(appdto.Bronze);
                Silver = BillingHelper.Round(appdto.Silver);
                Gold = BillingHelper.Round(appdto.Gold);
                Platinum = BillingHelper.Round(appdto.Platinum);
                ForecastBronze = BillingHelper.Round(appdto.ForecastBronze);
                ForecastSilver = BillingHelper.Round(appdto.ForecastSilver);
                ForecastGold = BillingHelper.Round(appdto.ForecastGold);
                ForecastPlatinum = BillingHelper.Round(appdto.ForecastPlatinum);
            }
        }
    }
}
