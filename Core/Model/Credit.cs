using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Credit : BaseEntity
    {
        public PhWallet SINWallet { get; set; }
        public LegalWallet ShopWallet { get; set; }
        public AbstractWallet OwnerWallet { get; set; }
        public AbstractWallet ScoringWallet { get; set; }
        public decimal CurrentStaticScoring { get; set; }
        public decimal CurrentDynamicScoring { get; set; }
        public SinLifeStyle CurrentSinLifeStyle { get; set; }
        public decimal Price { get; set; }
    }
}
