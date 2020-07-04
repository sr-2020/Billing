using Billing.DTO;
using Core.Model;
using InternalServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class QRDto
    {
        public SkuDto Sku { get; set; }
        public long QRID { get; set; }
        public int Shop { get; set; }
        public string QR { get; set; }
    }
}
