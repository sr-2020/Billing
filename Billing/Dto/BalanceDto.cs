using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class BalanceDto
    {
        public BalanceDto(SIN sin)
        {
            CurrentBalance = BillingHelper.Round(sin.Wallet.Balance);
            CurrentScoring = Math.Round(sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative, 2);
            SIN = sin.Passport.Sin;
            PersonName = sin.Passport.PersonName;
            Metatype = sin.Passport.Metatype?.Name ?? "неизвестно";
            Citizenship = sin.Passport.Citizenship ?? "неизвестно";
            Viza = sin.Passport.Viza ?? "неизвестно";
            Pledgee = sin.Passport.Mortgagee ?? "неизвестно";
            Id = ModelId;
        }
        public int Id { get; set; }
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
        public string Insurance { get; set; }
        public List<string> Licenses { get; set; }
    }
}
