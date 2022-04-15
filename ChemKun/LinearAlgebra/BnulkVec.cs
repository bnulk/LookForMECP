using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.LinearAlgebra
{
    partial class BnulkVec
    {
        public int dim;          //数组维数
        public double[] ele;

        #region 构造函数
        //构造函数
        public BnulkVec(int m)
        {
            //构造函数
            dim = m;
            ele = new double[dim];
            //用一维数组构造向量
        }

        public BnulkVec(double[] d)
        {
            //构造函数
            dim = d.Length;
            ele = new double[dim];
            for (int i=0;i<dim;i++)
            {
                ele[i] = d[i];
            }
        }
        #endregion 构造函数

        /// <summary>
        /// 向量加法重载
        /// 分量分别相加
        /// </summary>
        /// <param name="v1">第一个向量</param>
        /// <param name="v2">第二个向量</param>
        /// <returns>两个向量之和</returns>
        public static BnulkVec operator +(BnulkVec v1, BnulkVec v2)
        {
            //向量维数
            int n = v1.dim;
            //检查相加的两个向量维数是否相同
            if (n != v2.dim)
            {
                throw new IndexOutOfRangeException("向量维数不匹配。");
            }

            BnulkVec v3 = new BnulkVec(n);
            for (int i = 0; i < n; i++)
            {
                v3.ele[i] = v1.ele[i] + v2.ele[i];
            }

            return v3;
        }

        /// <summary>
        /// 向量减法重载
        /// 分量分别相减
        /// </summary>
        /// <param name="v1">第一个向量</param>
        /// <param name="v2">第二个向量</param>
        /// <returns>两个向量之差</returns>
        public static BnulkVec operator -(BnulkVec v1, BnulkVec v2)
        {
            //向量维数
            int n = v1.dim;
            //检查相加的两个向量维数是否相同
            if (n != v2.dim)
            {
                throw new IndexOutOfRangeException("向量维数不匹配。");
            }

            BnulkVec v3 = new BnulkVec(n);
            for (int i = 0; i < n; i++)
            {
                v3.ele[i] = v1.ele[i] - v2.ele[i];
            }

            return v3;
        }

        /// <summary>
        /// 向量数乘
        /// 分量分别乘以实数
        /// </summary>
        /// <param name="v1">向量</param>
        /// <param name="a">乘数</param>
        /// <returns>向量数乘之积</returns>
        public static BnulkVec operator *(BnulkVec v1, double a)
        {
            //向量维数
            int n = v1.dim;

            BnulkVec v2 = new BnulkVec(n);
            for (int i = 0; i < n; i++)
            {
                v2.ele[i] = v1.ele[i] * a;
            }

            return v2;
        }

        /// <summary>
        /// 向量数乘
        /// 分量分别乘以实数
        /// </summary>
        /// <param name="a">乘数</param>
        /// <param name="v1">向量</param>
        /// <returns>向量数乘之积</returns>
        public static BnulkVec operator *(double a, BnulkVec v1)
        {
            //向量维数
            int n = v1.dim;

            BnulkVec v2 = new BnulkVec(n);
            for (int i = 0; i < n; i++)
            {
                v2.ele[i] = v1.ele[i] * a;
            }

            return v2;
        }

        /// <summary>
        /// 向量数除
        /// 分量分别除以实数
        /// </summary>
        /// <param name="v1">向量</param>
        /// <param name="a">除数</param>
        /// <returns>向量数除之商</returns>
        public static BnulkVec operator /(BnulkVec v1, double a)
        {
            //向量维数
            int n = v1.dim;

            BnulkVec v2 = new BnulkVec(n);
            for (int i = 0; i < n; i++)
            {
                v2.ele[i] = v1.ele[i] / a;
            }

            return v2;
        }

        /// <summary>
        /// 标量积
        /// </summary>
        /// <param name="v1">第一个向量</param>
        /// <param name="v2">第二个向量</param>
        /// <returns>两个向量的标量积</returns>
        public static double operator |(BnulkVec v1, BnulkVec v2)
        {
            int m = v1.dim;
            int n = v2.dim;
            //检查相加的两个向量维数是否相同
            if (m != n)
            {
                throw new IndexOutOfRangeException("向量维数不匹配。");
            }

            double dotMul = 0.0;

            for (int i = 0; i < m; i++)
            {
                dotMul += v1.ele[i] * v2.ele[i];
            }

            return dotMul;
        }

        /// <summary>
        /// 张量积
        /// </summary>
        /// <param name="v1">第一个向量</param>
        /// <param name="v2">第二个向量</param>
        /// <returns>两个向量的张量积</returns>
        public static BnulkMatrix operator *(BnulkVec v1, BnulkVec v2)
        {
            int row = v1.dim;
            int column = v2.dim;
            //检查相加的两个向量维数是否相同
            if (row != column)
            {
                throw new IndexOutOfRangeException("向量维数不匹配。");
            }

            BnulkMatrix matrix = new BnulkMatrix(row, column);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrix.data[i, j] = v1.ele[i] * v2.ele[j];
                }
            }

            return matrix;
        }

        /// <summary>
        /// 负向量
        /// </summary>
        /// <param name="v1">向量</param>
        /// <returns>该向量的负向量</returns>
        public static BnulkVec operator -(BnulkVec v1)
        {
            int n = v1.dim;
            BnulkVec v2 = new BnulkVec(n);

            for (int i = 0; i < n; i++)
            {
                v2.ele[i] = -v1.ele[i];
            }

            return v2;
        }

        //------------- 向量模的平方
        public static double operator ~(BnulkVec v1)
        {
            int NO;

            //获取变量维数
            NO = v1.dim;
            double sum = 0.0;

            for(int i=0;i<NO;i++)
            {
                sum = sum + v1.ele[i] * v1.ele[i];
            }
            return sum;
        }

        //索引器
        public double this[int index]
        {
            get
            {
                //get accessor
                return ele[index];
            }
            set
            {
                //set accessor
                ele[index] = value;
            }
        }
    }
}
