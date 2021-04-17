using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class ShopCorporationContractDto
    {
        public ShopCorporationContractDto(Contract contract)
        {
            CorporationId = contract.CorporationId;
            CorporationName = contract.Corporation?.Name;
            ShopId = contract.ShopId;
            ShopName = contract.Shop?.Name;
            Status = ((ContractStatusEnum)contract.Status).ToString();
        }
        public int CorporationId { get; set; }
        public string CorporationName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string Status { get; set; }
        public int Contract { get; set; }
    }
}
