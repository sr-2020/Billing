using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class ShopViewModel
    {
        public ShopViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
