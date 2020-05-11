using Billing.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class NomenklaturaPage
    {
        public int ProductTypeId { get; set; }
        public List<NomenklaturaDto> Items { get; set; }
    }
}
