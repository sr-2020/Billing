using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CreateSkuRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NomenklaturaId { get; set; }
        public int Count { get; set; }
        public int Corporation { get; set; }
        public bool Enabled { get; set; }
    }
}
