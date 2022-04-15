using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.LinearAlgebra
{
    partial class BnulkMatrix
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2018-09-04

        描述：
            * 通过Householder镜像变换方法实现QR分解
        结构：
            * 
        方法：
            * qrdcmp(BnulkMatrix A, out BnulkMatrix Q, out BnulkMatrix R)
        代码来源：
            * C#科学计算讲义，P135
        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */

        public static void qrdcmp(BnulkMatrix A, out BnulkMatrix Q, out BnulkMatrix R)
        {
            int m = A.row;
            int n = A.column;

            Q = new BnulkMatrix(m, m);
            R = new BnulkMatrix(m, n);

            BnulkMatrix H0 = new BnulkMatrix(m, m);
            BnulkMatrix H1 = new BnulkMatrix(m, m);
            BnulkMatrix H2 = new BnulkMatrix(m, m);

            BnulkMatrix A1 = new BnulkMatrix(m, n);
            BnulkMatrix A2 = new BnulkMatrix(m, n);
            BnulkVec u = new BnulkVec(m);

            int i, j;
            double s, du;

            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                {
                    A1.data[i, j] = A.data[i, j];
                }
            }

            //设置H1为单位矩阵
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < m; j++)
                {
                    H1.data[i, j] = 0.0;
                }
            }
            for (j = 0; j < m; j++)
            {
                H1.data[j, j] = 1.0;
            }

            for (int k = 0; k < n; k++)
            //k表示所有的列
            {
                //设置H0为单位矩阵
                for (i = 0; i < m; i++)
                {
                    for (j = 0; j < m; j++)
                    {
                        H0.data[i, j] = 0.0;
                    }
                }
                for (j = 0; j < m; j++)
                {
                    H0.data[j, j] = 1.0;
                }

                s = 0.0;
                for (i = k; i < m; i++)
                {
                    s = s + A1[i, k] * A1[i, k];
                }
                s = Math.Sqrt(s);

                //这段甚为重要，关系到数值稳定性
                //目的是使u的范数尽可能大
                //原则是：如果首元素大于0，则u第一个分量为 正+正
                //如果首元素小于0，则u的第一个分量为 负+负 （更大的负数）
                for (i = 0; i < m; i++)
                {
                    u[i] = 0.0;
                }

                if (A[k, k] >= 0.0)
                {
                    u[k] = A1[k, k] + s;
                }
                else
                {
                    u[k] = A1[k, k] - s;
                }

                for (i = k + 1; i < m; i++)
                {
                    u[i] = A1[i, k];
                }

                //u的2范数平方，这里引用向量类的范数运算符重载
                du = ~u;

                //计算得到大的H矩阵
                for (i = k; i < m; i++)
                {
                    for (j = k; j < m; j++)
                    {
                        H0[i, j] = -2.0 * u[i] * u[j] / du;
                        if (i == j)
                        {
                            H0[i, j] = 1.0 + H0[i, j];
                        }
                    }
                }

                A2 = H0 * A1;

                for (i = 0; i < m; i++)
                {
                    for (j = 0; j < n; j++)
                    {
                        A1.data[i, j] = A2.data[i, j];
                    }
                }

                H1 = H1 * H0;
            }

            for (i = 0; i < m; i++)
            {
                for (j = 0; j < m; j++)
                {
                    Q.data[i, j] = H1.data[i, j];
                }
            }
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                {
                    R.data[i, j] = A1.data[i, j];
                }
            }

            return;
        }

    }
}
