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
        public QRDto(SkuDto sku, int shop)
        {
            Shop = shop;
            Sku = sku;
            QRID = QRHelper.Concatenate(sku.SkuId, shop);
            QR = EreminQrService.GetQRUrl(QRID);
        }
        public SkuDto Sku { get; set; }
        public long QRID { get; set; }
        public int Shop { get; set; }
        public string QR { get; set; }
    }
}
