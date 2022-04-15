using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Tools
{
    class Math_Kun
    {
        /// <summary>
        /// 保留n个有效数字的函数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double foo(double d, int n)
        {
            if (d == 0.0) return 0;
            if (d > 1 || d < -1)
                n = n - (int)Math.Log10(Math.Abs(d)) - 1;
            else
                n = n + (int)Math.Log10(1.0 / Math.Abs(d));
            if (n < 0)
            {
                d = (int)(d / Math.Pow(10, 0 - n)) * Math.Pow(10, 0 - n);
                n = 0;
            }
            return Math.Round(d, n);
        }
    }
}
