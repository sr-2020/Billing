using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class QRHelper
    {
        public static long Concatenate(int first, int second)
        {
            if(first < 0 || second < 0)
            {
                throw new Exception("value cannot be less 0");
            }
            long temp = first;
            long result = temp << 32;
            result += second;
            return result;
        }

        public static void Parse(long concatenated, out int first, out int second)
        {
            if(concatenated < 0)
            {
                throw new Exception("concatenated cannot be less 0");
            }
            long maxint = int.MaxValue;
            first = Convert.ToInt32(concatenated >> 32);
            second = Convert.ToInt32(maxint & concatenated);
        }

    }
}
