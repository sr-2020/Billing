using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class AbilityModel : BaseLocationBasedModel
    {
        public QrCodeModel QrCode { get; set; }
    }

    public class QrCodeModel
    {
        public int UsesLeft { get; set; }
        public string Pill { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ModelId { get; set; }
        public string EventType { get; set; }
        public RentaModel Data { get; set; }
    }

    public class RentaModel
    {
        public string Id { get; set; }
        public decimal BasePrice { get; set; }
        public decimal RentPrice { get; set; }
        public string DealId { get; set; }
        public string GmDescription { get; set; }
        public string LifeStyle { get; set; }
    }

}
