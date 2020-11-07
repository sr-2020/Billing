using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class HealthModel : BasePubSubModel
    {
        public int CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string StateFrom { get; set; }
        public string StateTo { get; set; }
    }
}
