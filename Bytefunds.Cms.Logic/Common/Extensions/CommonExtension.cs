
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class CommonExtension
    {
        public static DateTime ToDateTime(this string str)
        {
            DateTime result = new DateTime();
            DateTime.TryParse(str, out result);
            return result;
        }

        public static Double ToDouble(this string val)
        {
            Double result = 0;
            Double.TryParse(val, out result);
            return result;
        }

        public static int ToInt(this string val)
        {
            int result = 0;
            int.TryParse(val, out result);
            return result;
        }

        public static float Tofloat(this string val)
        {
            float result = 0;
            float.TryParse(val, out result);
            return result;
        }

        public static bool Tobool(this string val)
        {
            string[] truestr = { "true", "True", "TRUE", "1" };
            if (truestr.Contains(val))
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
