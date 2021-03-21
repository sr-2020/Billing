using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class CycleDto
    {
        public int LastCycle { get; set; }
        public int LastBeat { get; set; }
        public string CurrentToken { get; set; }
        public bool CycleIsActive { get; set; }

    }

    public class BeatDto
    {
        public string Comment { get; set; }
        public bool Success { get; set; }
        public string BeatAlias { get; set; }
    }

}
