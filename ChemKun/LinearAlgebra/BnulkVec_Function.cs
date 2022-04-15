using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.LinearAlgebra
{
    partial class BnulkVec
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2018-08-10

        描述：
            * 一些常用的向量方法。
        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */


        /// <summary>
        /// 向量的规范化：让向量的长度等于1
        /// </summary>
        /// <param name="v1">向量</param>
        /// <returns></returns>
        public static BnulkVec Normalize(BnulkVec v1)
        {
            int n = v1.dim;
            double length = 0;
            for (int i = 0; i < n; i++)
            {
                length += v1.ele[i] * v1.ele[i];
            }
            length = Math.Sqrt(length);
            for (int i = 0; i < n; i++)
            {
                v1.ele[i] = v1.ele[i] / length;
            }
            return v1;
        }

        /// <summary>
        /// 把向量转换成一维数组
        /// </summary>
        /// <param name="v1">向量</param>
        /// <returns>一维数组</returns>
        public static double[] ToArray(BnulkVec v1)
        {
            double[] array = new double[v1.dim];
            for(int i=0;i<v1.dim;i++)
            {
                array[i] = v1.ele[i];
            }
            return array;
        }
    }
}
