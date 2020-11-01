using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class BasePubSubModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TimeSpan timeSpan { get; set; }
        public string CharacterId { get; set; }
    }
}
