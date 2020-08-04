using Billing.Dto.Shop;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class ShopDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Comission { get; set; }
        public string Lifestyle { get; set; }
        public decimal Balance { get; set; }
        public int OwnerId { get; set; }
        public List<SpecialisationDto> Specialisations { get; set; }
    }
}
