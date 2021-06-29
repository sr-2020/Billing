using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class OrganisationBaseDto
    {
        public OrganisationBaseDto(int modelId, OwnerEntity organisation)
        {
            Id = organisation.Id;
            Name = organisation.Name;
            Owner = organisation.OwnerId ?? 0;
            IsOwner = Owner == modelId;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Owner { get; set; }
        public bool IsOwner { get; set; }
    }
}
