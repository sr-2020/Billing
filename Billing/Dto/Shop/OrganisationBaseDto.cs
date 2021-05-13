using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class OrganisationBaseDto
    {
        public OrganisationBaseDto(OwnerEntity organisation)
        {
            Id = organisation.Id;
            Name = organisation.Name;
            if (organisation.Owner == null)
                return;
            Owner = organisation.OwnerId ?? 0;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Owner { get; set; }
    }
}
