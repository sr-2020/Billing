using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class InitCharacterRequest
    {
        public string Name { get; set; }
        public List<string> Features { get; set; }
        public string Metarace { get; set; }
        public int Karma { get; set; }
    }
}
