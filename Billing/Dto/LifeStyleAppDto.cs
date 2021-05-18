using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class LifeStyleAppDto
    {
        public decimal Bronze { get; set; }
        public decimal ForecastBronze { get; set; }
        public decimal Silver { get; set; }
        public decimal ForecastSilver { get; set; }
        public decimal Gold { get; set; }
        public decimal ForecastGold { get; set; }
        public decimal Platinum { get; set; }
        public decimal ForecastPlatinum { get; set; }

        public Lifestyles GetLifeStyle(Wallet wallet)
        {
            if (wallet.IsIrridium)
                return Lifestyles.Iridium;
            return wallet.Balance > Platinum ? Lifestyles.Platinum : IsGold(wallet.Balance);
        }
        private Lifestyles IsGold(decimal balance)
        {
            return balance > Gold && balance < Platinum ? Lifestyles.Gold : IsSilver(balance);
        }
        private Lifestyles IsSilver(decimal balance)
        {
            return balance > Silver && balance < Gold ? Lifestyles.Silver : IsBronze(balance);
        }

        private Lifestyles IsBronze(decimal balance)
        {
            return balance > Bronze && balance < Silver ? Lifestyles.Bronze : Lifestyles.Wood;
        }

        public Lifestyles GetForecastLifeStyle(Wallet wallet)
        {
            if (wallet.IsIrridium)
                return Lifestyles.Iridium;
            var forecast = BillingHelper.GetForecast(wallet);
            return forecast > ForecastPlatinum ? Lifestyles.Platinum : IsForecastGold(forecast);
        }
        private Lifestyles IsForecastGold(decimal balance)
        {
            return balance > ForecastGold && balance < ForecastPlatinum ? Lifestyles.Gold : IsForecastSilver(balance);
        }
        private Lifestyles IsForecastSilver(decimal balance)
        {
            return balance > ForecastSilver && balance < ForecastGold ? Lifestyles.Silver : IsForecastBronze(balance);
        }

        private Lifestyles IsForecastBronze(decimal balance)
        {
            return balance > ForecastBronze && balance < ForecastSilver ? Lifestyles.Bronze : Lifestyles.Wood;
        }

    }
}
