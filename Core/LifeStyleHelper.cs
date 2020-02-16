using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class LifeStyleHelper
    {
        public static readonly Dictionary<Lifestyles, int> MaxValues = new Dictionary<Lifestyles, int>
        {
            { Lifestyles.Wood, 100 },
            { Lifestyles.Bronze, 200 },
            { Lifestyles.Silver, 300 },
            { Lifestyles.Gold, 400 },
            { Lifestyles.Platinum, 500 },
            { Lifestyles.Iridium, 600 },
        };

        public static Lifestyles GetLifeStyle(decimal balance)
        {
            foreach (var lifestyle in MaxValues)
            {
                if (balance < lifestyle.Value)
                    return lifestyle.Key;
            }
            return Lifestyles.Iridium;
        }

    }
}
