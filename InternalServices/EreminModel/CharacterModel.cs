using System;
using System.Collections.Generic;
using System.Text;

namespace InternalServices.EreminModel
{
    public class CharacterModel
    {
        public BaseModel baseModel { get; set; }
        public WorkModel workModel { get; set; }
    }
    public class Discounts
    {
        public string all { get; set; }
        public string samurai { get; set; }
    }
    public class Billing
    {
        public bool anonymous { get; set; }
        public decimal stockGainPercentage { get; set; }
    }
    public class BaseModel
    {
        public string modelId { get; set; }
        public Discounts discounts { get; set; }
        public Billing billing { get; set; }
    }
    public class WorkModel
    {
        public string modelId { get; set; }
        public Discounts discounts { get; set; }
        public Billing billing { get; set; }
    }

}
