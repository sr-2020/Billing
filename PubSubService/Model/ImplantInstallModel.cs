using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class ImplantInstallModel : BaseLocationModel
    {
        public string Id { get; set; }
        public string ImplantLifestyle { get; set; }
        public string AutodocLifestyle { get; set; }
    }
}
