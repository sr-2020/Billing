using Billing.Dto.Shop;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing.DTO
{
    public class ShopDto : OrganisationBaseDto
    {
        public ShopDto(ShopWallet shop):base(shop)
        {
            if (shop == null)
                return;
            Lifestyle = BillingHelper.GetLifestyle(shop.LifeStyle).ToString();
            var list = new List<int>();
            if (shop.Specialisations == null)
                Specialisations = list;
            list.AddRange(shop.Specialisations.Select(s => s.SpecialisationId));
            Specialisations = list;
            if (shop.Wallet == null)
                return;
            Balance = shop.Wallet.Balance;
        }
        public string Lifestyle { get; set; }
        public decimal Balance { get; set; }
        public List<int> Specialisations { get; set; }
    }
}
