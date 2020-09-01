using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class RentaDto
    {
        public int RentaId { get; set; }
        public decimal FinalPrice { get; set; }
        public string ProductType { get; set; }
        public string NomenklaturaName { get; set; }
        public string SkuName { get; set; }
        public string Corporation { get; set; }
        public string Shop { get; set; }
        public bool HasQRWrite { get; set; }
        public string QRRecorded { get; set; }
        public int PriceId { get; set; }
        public int ModelId { get; set; }
        public string CharacterName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
