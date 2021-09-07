using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExtension
    {
        public static int ToInt(this string s)
        {
            return Convert.ToInt32(s);
        }
    }
}
