using System;
using System.Collections.Generic;

namespace AutoCompleteBox
{
    class Compare : IEqualityComparer<String>
    {
        public bool Equals(String x, String y)
        {
            if (x.ToLower() == y.ToLower())
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(String codeh)
        {
            return 0;
        }
    }
}
