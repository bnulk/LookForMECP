using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.LinearAlgebra
{
    class CalcDet
    {
    /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2018-04-14

        描述：
            * 计算行列式的值
        结构：
            * 输入矩阵或者二维数组，ref输入输出行列式的值
        方法：
            * det(bnulkMatrix a, ref double d) -- 用LU分解中的Crout方法，先分解矩阵，再计算相应行列式的值
        ----------------------------------------------------  类注释  结束----------------------------------------------------
    */

        public void det(BnulkMatrix a, ref double d)
        {
            int N = a.rowNum;

            int i, j, k, r;
            BnulkMatrix L = new BnulkMatrix(N, N);
            BnulkMatrix U = new BnulkMatrix(N, N);

            //设置初值
            for(i=0;i<N;i++)
            {
                for(j=0;j<N;j++)
                {
                    L.data[i, j] = 0.0;
                    U.data[i, j] = 0.0;
                }
            }

            //L的第一列
            for (i = 0; i < N; i++)
            {
                L.data[i, 0] = a.data[i, 0];
            }

            //U的第一行
            for (i = 0; i < N; i++)
            {
                U.data[0, i] = a.data[0, i] / L.data[0, 0];
            }

            //临时变量
            double tmp = 0.0;

            //这一循环是核心，用于计算L的第k列，同时计算U的第k行
            //不可以分开循环，因为数据之间有相互依赖性
            for (k = 1; k < N; k++)
            {
                for(i = k; i < N; i++)
                {
                    tmp = 0.0;
                    for(r = 0; r <= k - 1; r++)
                    {
                        tmp = tmp + L.data[i, r] * U.data[r, k];
                    }
                    L.data[i, k] = a.data[i, k] - tmp;
                }

                for(j=k+1;j<N;j++)
                {
                    tmp = 0.0;
                    for(r=0;r<=k-1;r++)
                    {
                        tmp = tmp + L.data[k, r] * U.data[r, j];
                    }
                    U.data[k, j] = (a.data[k, j] - tmp) / L.data[k, k];
                }

                U.data[k, k] = 1.0;
            }

            //至此，已经计算出A=LU分解

            d = 1.0;

            for(i=0;i<N;i++)
            {
                d = d * L.data[i, i];
            }

            //完成计算

            return;
        }
    }
}
