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
        public ShopDto(ShopWallet shop) : base(shop)
        {
            if (shop == null)
                return;
            Lifestyle = new LifestyleDto
            {
                Id = shop.LifeStyle,
                Name = BillingHelper.GetLifestyle(shop.LifeStyle).ToString()
            };

            if (shop.Specialisations == null)
            {
                Specialisations = new List<int>();
            }
            else
            {
                Specialisations = shop.Specialisations.Select(s => s.SpecialisationId).ToList();
            }
            if(shop.TrustedUsers == null)
            {
                TrustedUsers = new List<int>();
            }
            else
            {
                TrustedUsers = shop.TrustedUsers.Select(s => s.Model).ToList();
            }
            if (shop.Wallet == null)
                return;
            Balance = shop.Wallet.Balance;
            Comment = shop.Comment;
            Location = shop.Location;
        }
        public LifestyleDto Lifestyle { get; set; }
        public decimal Balance { get; set; }
        public string Comment { get; set; }
        public string Location { get; set; }
        public List<int> Specialisations { get; set; }
        public List<int> TrustedUsers { get; set; }
    }
    public class ShopDetailedDto : ShopDto
    {
        public ShopDetailedDto(ShopWallet shop, List<QRDto> products) : base(shop) 
        {
            Products = products;
        }
        //List<>
        public List<QRDto> Products { get; set; }
    }
}
