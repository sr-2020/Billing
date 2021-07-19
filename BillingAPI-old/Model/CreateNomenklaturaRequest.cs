using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CreateNomenklaturaRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int SpecialisationId { get; set; }
        public int Lifestyle { get; set; }
        public decimal BasePrice { get; set; }
        public int BaseCount { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
    }
}
