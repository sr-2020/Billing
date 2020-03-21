using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class ShopDto
    {
        public int Id { get; set; }
        public int ForeignId { get; set; }
        public string Name { get; set; }
        public int Comission { get; set; }
        public int Lifestyle { get; set; }
    }
}
