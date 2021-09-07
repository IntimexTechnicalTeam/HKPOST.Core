using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ObjectExtension
    {
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }
    }
}
