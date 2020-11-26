using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Billing.Dto
{
    public class MainExcelDto
    {
        [Display(Name ="SIN")]
        public string ModelId { get; set; }
        [Display(Name = "Имя")]
        public string PersonName { get; set; }
        [Display(Name = "Баланс")]
        public decimal Balance { get; set; }
        [Display(Name = "Лайфстайл")]
        public string LifeStyle { get; set; }
        [Display(Name = "СумРентс")]
        public decimal SumRents { get; set; }
        //[Display(Name = "ИКАР")]
        //public decimal? Ikar { get; set; }
        //[Display(Name = "ИКАР коэф")]
        //public decimal IkarKoef { get; set; }
        [Display(Name = "Карма")]
        public decimal? Karma { get; set; }
        [Display(Name = "Карма коэф")]
        public decimal KarmaKoef { get; set; }
        [Display(Name = "Скоринг фикс")]
        public decimal ScoringFix { get; set; }
        [Display(Name = "Скоринг релатив")]
        public decimal ScoringRelative { get; set; }
        [Display(Name = "Инфляция коэф")]
        public decimal InflationKoef { get; set; }

    }
}
