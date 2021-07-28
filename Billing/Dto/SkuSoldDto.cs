using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class CorporationSkuSoldDto
    {
        public decimal SumCorpSkuSold { get; set; }
        public int CorporationId { get; set; }
        public string CorporationName { get; set; }
        public List<SpecialisationSkuSold> Specialisations { get; set; }
    }
    public class SpecialisationSkuSold 
    {
        public string SpecialisationName { get; set;        }
        public string NomenklaturaName { get;set; }
        public string SkuName { get; set; }
        public int Count { get; set; }
        public decimal ShopPriceSum { get; set; }
        public decimal HackedPriceSum { get; set; }
    }

}
