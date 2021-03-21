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
		public decimal weaponsArmor { get; set; }
        public decimal everything { get; set; }
        public decimal ares { get; set; }
        public decimal aztechnology { get; set; }
        public decimal saederKrupp { get; set; }
        public decimal spinradGlobal { get; set; }
        public decimal neonet1 { get; set; }
        public decimal evo { get; set; }
        public decimal horizon { get; set; }
        public decimal wuxing { get; set; }
        public decimal russia { get; set; }
        public decimal renraku { get; set; }
        public decimal mutsuhama { get; set; }
        public decimal shiavase { get; set; }

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
        public Karma karma { get; set; }
    }
    public class Karma
    {
        public decimal available { get; set; }
        public decimal spent { get; set; }
    }


}
