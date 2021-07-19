using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CreateSpecialisationRequest
    {
        public int SpecialisationId { get; set; }
        public int ProductTypeId { get; set; }
        public string Name { get; set; }
    }
}
