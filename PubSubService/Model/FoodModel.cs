using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class FoodModel : BaseLocationModel
    {
        public string Id { get; set; }
        public decimal BasePrice { get; set; }
        public decimal RentPrice { get; set; }
        public string DealId { get; set; }
        public string GmDescription { get; set; }
        public string LifeStyle { get; set; }
    }
}
