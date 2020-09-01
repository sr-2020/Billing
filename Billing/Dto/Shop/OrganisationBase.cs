using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class OrganisationBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public int OwnerName { get; set; }
    }
}
