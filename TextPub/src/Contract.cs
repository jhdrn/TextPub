using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub
{
    internal static class Contract
    {
        internal static void AssertNotNullOrWhitespace(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("");
        }

        internal static void AssertNotNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("");
        }
    }
}
