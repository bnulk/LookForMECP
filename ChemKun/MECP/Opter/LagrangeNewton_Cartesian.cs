using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ChemKun.Data;
using ChemKun.Estimate;

namespace ChemKun.MECP.Opter
{
    partial class LagrangeNewton_Cartesian
    {
        public LagrangeNewton_Cartesian(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            //初始化
            try
            {
                InitialWorkMatrix_Cartesian(data_Input, data_MECP, ref workMatrix_Cartesian);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.InitialWorkMatrix_Cartesian(data_Input) Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Cartesian Error" + "\n");
            }
            //计算
            try
            {
                GetNewX(data_Input, ref data_MECP);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.Calculate_Thread_EntryPoint Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Cartesian Error" + "\n");
            }
            return;
        }

        /// <summary>
        /// 初始化工作矩阵
        /// </summary>
        /// <param name="data_Input"></param>
        /// <param name="data_MECP"></param>
        private void InitialWorkMatrix_Cartesian(Data_Input data_Input, Data_MECP data_MECP, ref WorkMatrix_Cartesian workMatrix_Cartesian)
        {
            //初始化
            workMatrix_Cartesian.N = data_Input.gaussianInputSegment.N;
            //产生新的Gjf文件用
            workMatrix_Cartesian.Params = new string[3 * workMatrix_Cartesian.N];             //构型参数名字
            workMatrix_Cartesian.x = new double[3 * workMatrix_Cartesian.N];                  //Out文件中构型参数数值
            workMatrix_Cartesian.newX = new double[3 * workMatrix_Cartesian.N];             //新Gjf文件中构型参数数值
            workMatrix_Cartesian.strNewX = new string[3 * workMatrix_Cartesian.N];          //新Gjf文件中构型参数数值的字符串形式
            //定义两个二维数组读力常数矩阵，定义两个一维数组读力矩阵
            workMatrix_Cartesian.MatrixH1 = new double[3 * workMatrix_Cartesian.N, 3 * workMatrix_Cartesian.N];     //第一个态的力常数矩阵
            workMatrix_Cartesian.MatrixH2 = new double[3 * workMatrix_Cartesian.N, 3 * workMatrix_Cartesian.N];     //第二个态的力常数矩阵
            workMatrix_Cartesian.MatrixG1 = new double[3 * workMatrix_Cartesian.N];                                   //第一个态的力矩阵
            workMatrix_Cartesian.MatrixG2 = new double[3 * workMatrix_Cartesian.N];                                   //第二个态的力矩阵
            workMatrix_Cartesian.MatrixG1Params = new string[3 * workMatrix_Cartesian.N, 3];                          //第一个态的Out参量矩阵
            workMatrix_Cartesian.MatrixG2Params = new string[3 * workMatrix_Cartesian.N, 3];                          //第一个态的Out参量矩阵
            //定义三个计算中的矩阵
            workMatrix_Cartesian.DetParams_Z = new double[3 * workMatrix_Cartesian.N + 1];
            workMatrix_Cartesian.Omiga_Z = new double[3 * workMatrix_Cartesian.N + 1, 3 * workMatrix_Cartesian.N + 1];
            workMatrix_Cartesian.F_Z = new double[3 * workMatrix_Cartesian.N + 1];


            //把当前计算结果传递给workMatrix_Cartesian
            workMatrix_Cartesian.N = data_Input.gaussianInputSegment.N;
            workMatrix_Cartesian.Params = data_MECP.functionData.para;
            workMatrix_Cartesian.x = data_MECP.functionData.x;
            workMatrix_Cartesian.Energy1 = data_MECP.functionData.energy1;
            workMatrix_Cartesian.Energy2 = data_MECP.functionData.energy2;
            workMatrix_Cartesian.MatrixG1 = data_MECP.functionData.gradient1;
            workMatrix_Cartesian.MatrixG2 = data_MECP.functionData.gradient2;

            workMatrix_Cartesian.MatrixH1 = data_MECP.functionData.hessian1;
            workMatrix_Cartesian.MatrixH2 = data_MECP.functionData.hessian2;

            return;
        }

        //产生新的坐标
        private void GetNewX(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            //由能量，力矩阵和力常数矩阵产生F_Z列和Omiga_Z矩阵
            try
            {
                CalFZAndOmiga_Z(data_MECP.functionData.Lambda, data_MECP.functionData.N, workMatrix_Cartesian.Energy1, workMatrix_Cartesian.Energy2,
                workMatrix_Cartesian.MatrixG1, workMatrix_Cartesian.MatrixG2, workMatrix_Cartesian.MatrixH1, workMatrix_Cartesian.MatrixH2,
                ref workMatrix_Cartesian.F_Z, ref workMatrix_Cartesian.Omiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.CalFZAndOmiga_Z Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.CalFZAndOmiga_Z Error" + "\n");
            }
            //调用高斯-约当函数求方程的解
            try
            {
                CalGaussJordan(workMatrix_Cartesian.N, workMatrix_Cartesian.F_Z, workMatrix_Cartesian.Omiga_Z, ref workMatrix_Cartesian.DetParams_Z, ref workMatrix_Cartesian.tmpOmiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.CalGaussJordan Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.CalGaussJordan Error" + "\n");
            }
            //更新结果文件
            try
            {
                UpdateX(data_Input, workMatrix_Cartesian.DetParams_Z, ref data_MECP);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.UpdateX Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Cartesian.UpdateX Error" + "\n");
            }
        }

        /// <summary>
        /// 计算FZ和Omiga_Z
        /// </summary>
        /// <param name="Labuta"></param>
        /// <param name="N"></param>
        /// <param name="Energy1"></param>
        /// <param name="Energy2"></param>
        /// <param name="MatrixG1"></param>
        /// <param name="MatrixG2"></param>
        /// <param name="MatrixF1"></param>
        /// <param name="MatrixF2"></param>
        /// <param name="F_Z"></param>
        /// <param name="Omiga_Z"></param>
        public void CalFZAndOmiga_Z(double Labuta, int N, double Energy1, double Energy2, double[] MatrixG1, double[] MatrixG2, double[,] MatrixH1, double[,] MatrixH2, ref double[] F_Z, ref double[,] Omiga_Z)
        {
            for (int i = 0; i < 3 * N; i++)
            {
                F_Z[i] = Convert.ToDouble(MatrixG1[i] + Labuta * (MatrixG2[i] - MatrixG1[i]));
            }
            F_Z[3 * N] = Energy1 - Energy2;
            for (int i = 0; i < 3 * N; i++)
            {
                for (int j = 0; j < 3 * N; j++)
                {
                    Omiga_Z[i, j] = (1 - Labuta) * MatrixH1[i, j] + Labuta * MatrixH2[i, j];
                }
            }
            for (int i = 0; i < 3 * N; i++)
            {
                Omiga_Z[i, 3 * N] = MatrixG1[i] - MatrixG2[i];
            }
            for (int i = 0; i < 3 * N; i++)
            {
                Omiga_Z[3 * N, i] = MatrixG1[i] - MatrixG2[i];
            }
            Omiga_Z[3 * N, 3 * N] = 0;
            return;
        }

        //调用高斯-约当函数求方程的解
        public void CalGaussJordan(int N, double[] F_Z, double[,] Omiga_Z, ref double[] DetParams_Z, ref double[] tmpOmiga_Z)
        {
            //DetParams_Z = F_Z;                           //当心！这样赋值就把F_Z的地址给DetParams_Z了，F_Z和DetParams_Z将变成一个数组了。所以废弃掉这么写了。
            DetParams_Z = new double[3 * N + 1];
            for (int i = 0; i < 3 * N + 1; i++)
            {
                DetParams_Z[i] = F_Z[i];
            }
            tmpOmiga_Z = new double[(3 * N + 1) * (3 * N + 1)];
            for (int i = 0; i < 3 * N + 1; i++)
            {
                for (int j = 0; j < 3 * N + 1; j++)
                {
                    tmpOmiga_Z[i * (3 * N + 1) + j] = Omiga_Z[i, j];
                }
            }
            LinearAlgebra.LineEquation.gaussj(ref tmpOmiga_Z, 3 * N + 1, ref DetParams_Z);  //调用函数
        }

        //更新结果文件
        public void UpdateX(Data_Input data_Input, double[] DetParams_Z, ref Data_MECP data_MECP)
        {
            //给Lambda赋值
            data_MECP.functionData.Lambda = data_MECP.functionData.Lambda + DetParams_Z[3 * workMatrix_Cartesian.N] * data_MECP.stepSize;
            //更新I
            data_MECP.I++;
            //更新x
            for (int i = 0; i < DetParams_Z.Length - 1; i++)
            {
                data_MECP.newX[i] = data_MECP.functionData.x[i] + DetParams_Z[i] * data_MECP.stepSize;
            }
            return;
        }
    }
}
