using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Tools
{
    public class Statistics_Kun
    {
        public Statistics_Kun()
        {

        }

        /// <summary>
        /// 返回数组中的最大值
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>最大值</returns>
        public static double MaxArray(double[] array)
        {
            double max = 0;
            for(int i=0;i<array.Length;i++)
            {
                array[i] = Math.Abs(array[i]);
                if (max < array[i])
                    max = array[i];
            }
            return max;
        }

        /// <summary>
        /// 返回数组中所有数的均方根
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>均方根</returns>
        public static double RMSArray(double[] array)
        {
            double RMS = 0;
            for(int i=0;i<array.Length;i++)
            {
                RMS += array[i] * array[i];
            }
            RMS = RMS / array.Length;
            RMS = Math.Pow(RMS, 0.5);
            return RMS;
        }

    }
}
