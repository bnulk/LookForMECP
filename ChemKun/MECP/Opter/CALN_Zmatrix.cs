using System;
using ChemKun.Data;

namespace ChemKun.MECP.Opter
{
    partial class CALN_Zmatrix
    {
        public CALN_Zmatrix(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            //初始化
            try
            {
                InitialWorkMatrix_Zmatrix(data_Input, data_MECP, ref workMatrix_ZMatrix);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.InitialWorkMatrix_Zmatrix(data_Input) Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix Error" + "\n");
            }
            //计算
            try
            {
                GetNewX(data_Input, ref data_MECP);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.Calculate_Thread_EntryPoint Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix Error" + "\n");
            }
            return;
        }

        /// <summary>
        /// 初始化工作矩阵
        /// </summary>
        /// <param name="data_Input"></param>
        /// <param name="data_MECP"></param>
        private void InitialWorkMatrix_Zmatrix(Data_Input data_Input, Data_MECP data_MECP, ref WorkMatrix_ZMatrix workMatrix_ZMatrix)
        {
            //初始化
            workMatrix_ZMatrix.N = data_Input.gaussianInputSegment.N;
            //产生新的Gjf文件用
            workMatrix_ZMatrix.Params = new string[3 * workMatrix_ZMatrix.N - 6];             //构型参数名字
            workMatrix_ZMatrix.x = new double[3 * workMatrix_ZMatrix.N - 6];                  //Out文件中构型参数数值
            workMatrix_ZMatrix.newX = new double[3 * workMatrix_ZMatrix.N - 6];             //新Gjf文件中构型参数数值
            workMatrix_ZMatrix.strNewX = new string[3 * workMatrix_ZMatrix.N - 6];          //新Gjf文件中构型参数数值的字符串形式
            //定义两个二维数组读力常数矩阵，定义两个一维数组读力矩阵
            workMatrix_ZMatrix.MatrixH1 = new double[3 * workMatrix_ZMatrix.N - 6, 3 * workMatrix_ZMatrix.N - 6];     //第一个态的力常数矩阵
            workMatrix_ZMatrix.MatrixH2 = new double[3 * workMatrix_ZMatrix.N - 6, 3 * workMatrix_ZMatrix.N - 6];     //第二个态的力常数矩阵
            workMatrix_ZMatrix.MatrixG1 = new double[3 * workMatrix_ZMatrix.N - 6];                                   //第一个态的力矩阵
            workMatrix_ZMatrix.MatrixG2 = new double[3 * workMatrix_ZMatrix.N - 6];                                   //第二个态的力矩阵
            workMatrix_ZMatrix.MatrixG1Params = new string[3 * workMatrix_ZMatrix.N - 6, 3];                          //第一个态的Out参量矩阵
            workMatrix_ZMatrix.MatrixG2Params = new string[3 * workMatrix_ZMatrix.N - 6, 3];                          //第一个态的Out参量矩阵
            //定义三个计算中的矩阵
            workMatrix_ZMatrix.DetParams_Z = new double[3 * workMatrix_ZMatrix.N - 5];
            workMatrix_ZMatrix.Omiga_Z = new double[3 * workMatrix_ZMatrix.N - 5, 3 * workMatrix_ZMatrix.N - 5];
            workMatrix_ZMatrix.F_Z = new double[3 * workMatrix_ZMatrix.N - 5];


            //把当前计算结果传递给workMatrix_ZMatrix
            workMatrix_ZMatrix.N = data_Input.gaussianInputSegment.N;
            workMatrix_ZMatrix.Params = data_MECP.functionData.para;
            workMatrix_ZMatrix.x = data_MECP.functionData.x;
            workMatrix_ZMatrix.Energy1 = data_MECP.functionData.energy1;
            workMatrix_ZMatrix.Energy2 = data_MECP.functionData.energy2;
            workMatrix_ZMatrix.MatrixG1 = data_MECP.functionData.gradient1;
            workMatrix_ZMatrix.MatrixG2 = data_MECP.functionData.gradient2;

            workMatrix_ZMatrix.MatrixH1 = data_MECP.functionData.hessian1;
            workMatrix_ZMatrix.MatrixH2 = data_MECP.functionData.hessian2;

            return;
        }

        //产生新的坐标
        private void GetNewX(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            double lambda1, lambda2;

            //第一遍
            try  //由能量，力矩阵和力常数矩阵产生F_Z列和Omiga_Z矩阵
            {
                CalFZAndOmiga_Z(data_MECP.functionData.Lambda, data_MECP.functionData.N, workMatrix_ZMatrix.Energy1, workMatrix_ZMatrix.Energy2,
                workMatrix_ZMatrix.MatrixG1, workMatrix_ZMatrix.MatrixG2, workMatrix_ZMatrix.MatrixH1, workMatrix_ZMatrix.MatrixH2,
                ref workMatrix_ZMatrix.F_Z, ref workMatrix_ZMatrix.Omiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalFZAndOmiga_Z Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalFZAndOmiga_Z Error" + "\n");
            }

            try  //调用高斯-约当函数求方程的解
            {
                CalGaussJordan(workMatrix_ZMatrix.N, workMatrix_ZMatrix.F_Z, workMatrix_ZMatrix.Omiga_Z, ref workMatrix_ZMatrix.DetParams_Z_1, ref workMatrix_ZMatrix.tmpOmiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalGaussJordan Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalGaussJordan Error" + "\n");
            }
            //给Lambda赋值
            lambda1 = data_MECP.functionData.Lambda + workMatrix_ZMatrix.DetParams_Z_1[3 * workMatrix_ZMatrix.N - 6] * data_MECP.stepSize;


            //第二遍辅助
            try  //由能量，力矩阵和力常数矩阵产生F_Z列和Omiga_Z矩阵
            {
                CalFZAndOmiga_Z(1-data_MECP.functionData.Lambda, data_MECP.functionData.N, workMatrix_ZMatrix.Energy2, workMatrix_ZMatrix.Energy1,
                workMatrix_ZMatrix.MatrixG2, workMatrix_ZMatrix.MatrixG1, workMatrix_ZMatrix.MatrixH2, workMatrix_ZMatrix.MatrixH1,
                ref workMatrix_ZMatrix.F_Z, ref workMatrix_ZMatrix.Omiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalFZAndOmiga_Z Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalFZAndOmiga_Z Error" + "\n");
            }
            

            try  //调用高斯-约当函数求方程的解
            {
                CalGaussJordan(workMatrix_ZMatrix.N, workMatrix_ZMatrix.F_Z, workMatrix_ZMatrix.Omiga_Z, ref workMatrix_ZMatrix.DetParams_Z_2, ref workMatrix_ZMatrix.tmpOmiga_Z);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalGaussJordan Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.CalGaussJordan Error" + "\n");
            }

            lambda2 = (1 - data_MECP.functionData.Lambda) + workMatrix_ZMatrix.DetParams_Z_2[3 * workMatrix_ZMatrix.N - 6] * data_MECP.stepSize;


            //更新结果文件
            for (int i=0;i<workMatrix_ZMatrix.DetParams_Z.Length;i++)
            {
                workMatrix_ZMatrix.DetParams_Z[i] = (workMatrix_ZMatrix.DetParams_Z_1[i] + workMatrix_ZMatrix.DetParams_Z_2[i]) / 2;
            }
            


            try
            {
                data_MECP.functionData.Lambda = lambda1 / (lambda1 + lambda2);
                UpdateX(data_Input, workMatrix_ZMatrix.DetParams_Z, ref data_MECP);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.UpdateX Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Opter.LagrangeNewton_Zmatrix.UpdateX Error" + "\n");
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
            for (int i = 0; i < 3 * N - 6; i++)
            {
                F_Z[i] = Convert.ToDouble(MatrixG1[i] + Labuta * (MatrixG2[i] - MatrixG1[i]));
            }
            F_Z[3 * N - 6] = Energy1 - Energy2;
            for (int i = 0; i < 3 * N - 6; i++)
            {
                for (int j = 0; j < 3 * N - 6; j++)
                {
                    Omiga_Z[i, j] = (1 - Labuta) * MatrixH1[i, j] + Labuta * MatrixH2[i, j];
                }
            }
            for (int i = 0; i < 3 * N - 6; i++)
            {
                Omiga_Z[i, 3 * N - 6] = MatrixG1[i] - MatrixG2[i];
            }
            for (int i = 0; i < 3 * N - 6; i++)
            {
                Omiga_Z[3 * N - 6, i] = MatrixG1[i] - MatrixG2[i];
            }
            Omiga_Z[3 * N - 6, 3 * N - 6] = 0;
            return;
        }

        //调用高斯-约当函数求方程的解
        public void CalGaussJordan(int N, double[] F_Z, double[,] Omiga_Z, ref double[] DetParams_Z, ref double[] tmpOmiga_Z)
        {
            //DetParams_Z = F_Z;                           //当心！这样赋值就把F_Z的地址给DetParams_Z了，F_Z和DetParams_Z将变成一个数组了。所以废弃掉这么写了。
            DetParams_Z = new double[3 * N - 5];
            for (int i = 0; i < 3 * N - 5; i++)
            {
                DetParams_Z[i] = F_Z[i];
            }
            tmpOmiga_Z = new double[(3 * N - 5) * (3 * N - 5)];
            for (int i = 0; i < 3 * N - 5; i++)
            {
                for (int j = 0; j < 3 * N - 5; j++)
                {
                    tmpOmiga_Z[i * (3 * N - 5) + j] = Omiga_Z[i, j];
                }
            }
            LinearAlgebra.LineEquation.gaussj(ref tmpOmiga_Z, 3 * N - 5, ref DetParams_Z);  //调用函数
        }

        //更新结果文件
        public void UpdateX(Data_Input data_Input, double[] DetParams_Z, ref Data_MECP data_MECP)
        {
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
