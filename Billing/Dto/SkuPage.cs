using Billing.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class SkuPage
    {
        public int NomenklaturaId { get; set; }
        public string NomenklaturaName { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public List<SkuDto> Items { get; set; }
    }
}
