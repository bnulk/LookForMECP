using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;

namespace ChemKun.MECP
{
    class Data_MECP
    {
        public string scfMethod1;
        public string scfMethod2;
        public bool isHessian;                           //是否加freq
        public int I;                                    //第I次计算的结果
        public double stepSize;                          //步长
        public bool isConvergence;                       //是否收敛
        public string mecpFreq;                          //振动分析选项
        public bool isReadFirst;                         //是否读第一步的计算数据

        public double[] newX;                            //新坐标数组

        public struct FunctionData
        {
            public int I;                                //第I次计算的结果，为历史数据提供信息。
            public int N;                                //原子个数
            public string coordinateType;                //坐标类型
            public double Lambda;
            public double energy1;
            public string[] para;
            public double[] x;                           //读计算结果得到，所以说原子单位。
            public double[] gradient1;
            public double[,] hessian1;
            public double energy2;
            public double[] gradient2;
            public double[,] hessian2;
        }
        public FunctionData functionData;
        public List<FunctionData> historyFunctionData = new List<FunctionData>();
        public List<List<string>> record = new List<List<string>>();

        /// <summary>
        /// 猜测更新Hessian阵用到的结构
        /// </summary>
        public struct EstimateHessian
        {
            public int dim;
            public double[] lastMatrixX1, lastMatrixX2;        //旧的坐标矩阵。
            public double[] lastMatrixG1, lastMatrixG2;        //旧的力矩阵。
            public double[,] lastMatrixH1, lastMatrixH2;       //旧的力常数矩阵，二维数组。
            public double[] matrixX1, matrixX2;                //坐标矩阵。
            public double[] matrixG1, matrixG2;                //力矩阵行。
        }
        public EstimateHessian estimateHessian;

        /// <summary>
        /// 加边Hessian
        /// </summary>
        public struct Freq
        {
            public bool isFreq;                                //是否做Freq计算
            public bool isRealMECP;                            //是否为真正极小
            public int dim;                                    //加边矩阵的维度
            public double[,] oldBorderedHessian;               //老的加边Hessian矩阵
            public double[,] borderedHessian;                  //加边Hessian矩阵
            public double[] principalMinors;                   //各个主子式的值
            public double[] diagonalBorderedHessian;           //对角化加边Hessian矩阵
            public BnulkVec[] eigenVecBorderedHessian;         //加边Hessian矩阵的本征向量

            public BnulkVec gradientConstrainedConditions;
            public BnulkMatrix L;
            public BnulkMatrix E;
            public BnulkMatrix EtLE;
            public BnulkVec eigenValueEtLE;
            public BnulkVec[] eigenVecEtLE;
        }
        public Freq freq;

        public struct LiuFreq
        {
            public int N;                                //原子个数
            public int[] atomicNumber;                   //原子序数
            public int[] atomicType;                     //原子类型
            public BnulkMatrix x;                        //笛卡尔坐标
            public double Lambda;                        //拉格朗日因子
            public double energy1;
            public BnulkVec gradient1;
            public BnulkMatrix hessian1;                   
            public double energy2;
            public BnulkVec gradient2;
            public BnulkMatrix hessian2;
            public bool isRealMECP;                      //是否为真正极小
        }
        public LiuFreq liuFreq;

        public struct Criteria
        {
            public double deltaEnergy;
            public double[] lagrangeForce;
            public double maxLagrangeForce;
            public double RMSLagrangeForce;
            public double criteriaEnergy;
            public double criteriaMax;
            public double criteriaRMS;
        }
        public Criteria criteria;
    }
}
