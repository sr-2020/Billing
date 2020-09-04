using System;
using System.Collections.Generic;
using System.Text;

namespace InternalServices.EreminModel
{
    public class QRDecodedModel
    {
        public int Type { get; set; }
        public int Kind { get; set; }
        public TimeSpan ValidUntil { get; set; }
        public string Payload { get; set; }
    }
}
