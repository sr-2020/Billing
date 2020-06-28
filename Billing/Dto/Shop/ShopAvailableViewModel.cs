using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class ShopAvailableViewModel : ShopViewModel
    {
        public ShopAvailableViewModel(List<QRDto> items, int id, string name)
            :base(id, name)
        {
            Items = items;
        }
        public List<QRDto> Items { get; set; }
    }
}
