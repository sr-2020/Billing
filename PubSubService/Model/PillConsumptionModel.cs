using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class PillConsumptionModel : BaseLocationModel
    {
        public string Id { get; set; } // айди препарата
        public string LifeStyle { get; set; }// лайфстайл препарата
    }
}
