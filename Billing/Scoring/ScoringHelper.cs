using Billing;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scoringspace
{
    public class ScoringHelper
    {
        public static int GetEventNumberLifestyle(Lifestyles from, Lifestyles to)
        {
            return (int)from * 10 + (int)to;
        }
    }
}
