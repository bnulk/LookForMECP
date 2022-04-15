using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.MECP.Opter
{
    partial class LagrangeNewton_Cartesian
    {
        // <summary>
        /// z矩阵的工作矩阵
        /// </summary>
        public struct WorkMatrix_Cartesian
        {
            public int N;                              //分子中原子个数
            public double Energy1;                     //第一个态能量
            public double Energy2;                     //第二个态能量

            //产生新的Gjf文件用
            public string[] Params;                    //构型参数名字，3N行。
            public double[] x;                         //输出文件中构型参数数值，3N行。
            public double[] newX;                      //新Gjf文件中构型参数数值，3N行。
            public string[] strNewX;                   //新Gjf文件中构型参数数值的字符串形式，3N行。
            //定义两个二维数组读力常数矩阵，定义两个一维数组读力矩阵
            public double[,] MatrixH1, MatrixH2;       //力常数矩阵，二维数组，(3N)*(3N)。
            public string[,] MatrixG1Params, MatrixG2Params;   //力参数矩阵，Out文件中输出的(3N)*3矩阵
            public double[] MatrixG1, MatrixG2;        //力矩阵，3N行。
            //定义三个计算中的矩阵
            public double[,] Omiga_Z;                  //ω阵，和力常数以及梯度相关，二维数组，(3N+1)*(3N+1)。详见附录文本。
            public double[] tmpOmiga_Z;                //ω阵的逆矩阵。
            public double[] F_Z;                       //F_Z阵，3N+1行。前3N行是梯度，最后一行是E1-E2。
            public double[] DetParams_Z;               //参数的Det值，3N+1行。其中前3N是构型参数，最后一行是拉格朗日λ值。
        }
        public static WorkMatrix_Cartesian workMatrix_Cartesian;


    }
}
