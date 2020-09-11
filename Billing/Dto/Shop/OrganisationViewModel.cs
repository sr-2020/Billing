using Billing.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class OrganisationViewModel
    {
        public int CurrentModelId { get; set; }
        public string CurrentCharacterName { get; set; }
        public List<ShopDto> Shops { get; set; }
        public List<CorporationDto> Corporations { get; set; }
    }
}
