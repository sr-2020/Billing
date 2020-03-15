using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sku")]
    public class Sku : BaseEntity
    {

        public int CorporationId { get; set; }
        public CorporationWallet Corporation { get; set; }
        
        public int ShopId { get; set; }
        public ShopWallet Shop { get; set; }
    }
}
