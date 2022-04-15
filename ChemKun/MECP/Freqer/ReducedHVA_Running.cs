using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;
using ChemKun.Output;

namespace ChemKun.MECP.Freqer
{
    partial class ReducedHVA
    {
        #region 变量
        /// <summary>
        /// 拉格朗日函数梯度
        /// </summary>
        BnulkVec gradient;
        /// <summary>
        /// 拉格朗日函数力常数
        /// </summary>
        BnulkMatrix hessian;

        /// <summary>
        /// 质权笛卡尔力常数矩阵
        /// </summary>
        BnulkMatrix Fm;
        /// <summary>
        /// 包含平转振约束四种频率
        /// </summary>
        public BnulkVec fullFrequencies;
        /// <summary>
        /// 平转振三种模式
        /// </summary>
        public BnulkMatrix fullMode;
        /// <summary>
        /// 振动频率
        /// </summary>
        public BnulkVec vibrationalFrequencies;
        /// <summary>
        /// 振动模式
        /// </summary>
        public BnulkMatrix vibrationalMode;

        /// <summary>
        /// 振动个数
        /// </summary>
        int numberOfVibration;
        /// <summary>
        /// 原点在质心的分子笛卡尔坐标
        /// </summary>
        BnulkMatrix coordinateOfMassCenter;
        /// <summary>
        /// 主惯性矩本征值
        /// </summary>
        BnulkVec principalMoments;
        /// <summary>
        /// 主惯性矩向量
        /// </summary>
        BnulkMatrix X;
        /// <summary>
        /// D矩阵
        /// </summary>
        BnulkMatrix fullD;
        /// <summary>
        /// D矩阵的振动子矩阵
        /// </summary>
        BnulkMatrix D;
        /// <summary>
        /// 内坐标系下的力常数矩阵
        /// </summary>
        BnulkMatrix fullFint;
        /// <summary>
        /// 内坐标系下的振动力常数矩阵
        /// </summary>
        BnulkMatrix Fint;
        /// <summary>
        /// 内坐标下完整的力常数矩阵本征值
        /// </summary>
        BnulkVec fullEigenLambda;
        /// <summary>
        /// 内坐标下力常数矩阵本征值
        /// </summary>
        BnulkVec eigenLambda;
        /// <summary>
        /// 内坐标下力常数矩阵完整的本征向量，即原子位移L矩阵
        /// </summary>
        BnulkMatrix fullL;
        /// <summary>
        /// 内坐标下力常数矩阵本征向量，即原子位移L矩阵
        /// </summary>
        BnulkMatrix L;
        /// <summary>
        /// 简正坐标下分子平转振动的归一化因子向量
        /// </summary>
        BnulkVec fullNormalizationFactor;
        /// <summary>
        /// 简正坐标下分子振动的归一化因子向量
        /// </summary>
        BnulkVec normalizationFactor;
        /// <summary>
        /// D矩阵的互补集
        /// </summary>
        BnulkMatrix complementarySetOfD;
        /// <summary>
        /// 是否为真正的势能面极小交叉点
        /// </summary>
        bool isRealMECP;
        #endregion 变量

        public void Running()
        {
            //初始化计算
            Initialize();
            //显示计算初始值
            if(N<6)
            {
                ShowInitialValue();
            }            

            //计算质权笛卡尔力常数矩阵
            Cart2MWC(out Fm);
            if(N<6)
            {
                WriteOutput.ShowMatrix(Fm, "Fm");
            }
            
            //获取质心为原点的坐标
            GetCoordinateOfMassCenter(out coordinateOfMassCenter);
            //获得主惯性矩本征值及其向量
            GetPrincipalMomentsAndEigenVec(out principalMoments, out X);
            if (N < 6)
            {
                WriteOutput.ShowVector(principalMoments, "principalMoments");
                WriteOutput.ShowMatrix(X, "X");
            }
            
            //获取振动的个数
            GetNumberOfVibration(out numberOfVibration);
            //获取Sayvetz条件的D矩阵
            GetSayvetzMatrix(out fullD);
            if (N < 6)
            {
                WriteOutput.ShowMatrix(fullD, "fullD");
            }
            
            //获取内坐标系下的力常数矩阵
            GetFullFint(out Fint, out fullFint);
            if (N < 6)
            {
                WriteOutput.ShowMatrix(Fint, "Fint");
            }
            
            //对角化内坐标下力常数矩阵
            DiagFullFint(out fullEigenLambda, out fullL);
            if (N < 6)
            {
                WriteOutput.ShowMatrix(fullL, "fullL");
            }
            
            //把力常数本征值，转换为波数单位的频率
            CalVibrationalFrequencies(out fullFrequencies);
            //获取笛卡尔坐标下的振动向量
            GetCartesianFullL(out fullNormalizationFactor, out fullMode);
            if (N < 6)
            {
                WriteOutput.ShowVector(fullFrequencies, "fullFrequencies");
                WriteOutput.ShowMatrix(fullMode, "fullMode");
            }            

            ToVibrationalData(out vibrationalFrequencies, out vibrationalMode);
            //排序
            SortingHVA(ref vibrationalFrequencies, ref vibrationalMode);

            //判定是否为真正极小势能面交叉点
            isRealMECP = IsRealMECP(vibrationalFrequencies);
            //以文本形式输出计算结果
            WriteOutput.VibrationalAnalysis(atomicNumbers, vibrationalFrequencies, vibrationalMode);

            return;
        }
    }
}
