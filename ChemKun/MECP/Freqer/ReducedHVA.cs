using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;
using ChemKun.Output;
using ChemKun.FundamentalConstants;
using ChemKun.MECP;

namespace ChemKun.MECP.Freqer
{
    partial class ReducedHVA
    {
        #region 变量
        /// <summary>
        /// 原子个数
        /// </summary>
        readonly int N;
        /// <summary>
        /// 原子序号（核电荷数）数组
        /// </summary>
        readonly int[] atomicNumbers;
        /// <summary>
        /// 原子量数组
        /// </summary>
        readonly double[] realAtomicWeights;
        /// <summary>
        /// 拉格朗日因子
        /// </summary>
        readonly double lambda;
        /// <summary>
        /// 分子中原子的笛卡尔坐标
        /// </summary>
        readonly BnulkMatrix x;
        /// <summary>
        /// 第一个态的能量
        /// </summary>
        readonly double energy1;
        /// <summary>
        /// 第二个态的能量
        /// </summary>
        readonly double energy2;
        /// <summary>
        /// 第一个态的梯度
        /// </summary>
        readonly BnulkVec gradient1;
        /// <summary>
        /// 第二个态的梯度
        /// </summary>
        readonly BnulkVec gradient2;
        /// <summary>
        /// 第一个态的力常数
        /// </summary>
        readonly BnulkMatrix hessian1;
        /// <summary>
        /// 第一个态的力常数
        /// </summary>
        readonly BnulkMatrix hessian2;
        #endregion 变量

        public ReducedHVA(Data_MECP.LiuFreq liuFreq)
        {
            int i, j;
            N = liuFreq.N;
            int dim = 3 * N;

            atomicNumbers = new int[N];
            realAtomicWeights = new double[N];
            x = new BnulkMatrix(N, 3);
            gradient1 = new BnulkVec(dim);
            gradient2 = new BnulkVec(dim);
            hessian1 = new BnulkMatrix(dim, dim);
            hessian2 = new BnulkMatrix(dim, dim);

            for (i=0;i<N;i++)
            {
                atomicNumbers[i] = liuFreq.atomicNumber[i];
                realAtomicWeights[i] = Masses.NumberToMass(atomicNumbers[i]);
            }
            lambda = liuFreq.Lambda;
            for(i=0;i<N;i++)
            {
                for(j=0;j<3;j++)
                {
                    x[i, j] = liuFreq.x[i, j];
                }
            }
            energy1 = liuFreq.energy1;
            energy2 = liuFreq.energy2;
            for (i = 0; i < dim; i++)
            {
                gradient1[i] = liuFreq.gradient1[i];
                gradient2[i] = liuFreq.gradient2[i];
                for(j=0;j<dim;j++)
                {
                    hessian1[i, j] = liuFreq.hessian1[i, j];
                    hessian2[i, j] = liuFreq.hessian2[i, j];
                }
            }
        }

        /// <summary>
        /// 初始化拉格朗日梯度和拉格朗日Hessian
        /// </summary>
        private void Initialize()
        {
            int i, j;
            int dim = 3 * N;
            gradient = new BnulkVec(dim);
            hessian = new BnulkMatrix(dim, dim);

            for (i=0; i < dim; i++)
            {
                gradient[i] = gradient1[i] - lambda * (gradient1[i] - gradient2[i]);
                for (j = 0; j < dim; j++)
                {
                    hessian[i,j] = hessian1[i,j] - lambda * (hessian1[i,j] - hessian2[i,j]);
                }
            }
            return;
        }

        /// <summary>
        /// 计算质权笛卡尔力常数矩阵
        /// </summary>
        /// <param name="Fm">笛卡尔质权力常数矩阵</param>
        private void Cart2MWC(out BnulkMatrix Fm)
        {
            int dim = 3 * N;
            Fm = new BnulkMatrix(dim, dim);
            double sqrtMM = 0.0;
            int I, J;

            for (int i = 0; i < N; i++)
            {
                I = 3 * i;

                sqrtMM = Math.Sqrt(realAtomicWeights[i] * realAtomicWeights[i]);
                Fm[I, I] = hessian[I, I] / sqrtMM;
                Fm[I + 1, I + 1] = hessian[I + 1, I + 1] / sqrtMM;
                Fm[I + 2, I + 2] = hessian[I + 2, I + 2] / sqrtMM;
                Fm[I, I + 1] = Fm[I + 1, I] = hessian[I, I + 1] / sqrtMM;
                Fm[I, I + 2] = Fm[I + 2, I] = hessian[I, I + 2] / sqrtMM;
                Fm[I + 1, I + 2] = Fm[I + 2, I + 1] = hessian[I + 1, I + 2] / sqrtMM;

                for (int j = 0; j < i; j++)
                {
                    J = 3 * j;
                    sqrtMM = Math.Sqrt(realAtomicWeights[i] * realAtomicWeights[j]);

                    Fm[I, J] = Fm[J, I] = hessian[I, J] / sqrtMM;
                    Fm[I, J + 1] = Fm[J + 1, I] = hessian[I, J + 1] / sqrtMM;
                    Fm[I, J + 2] = Fm[J + 2, I] = hessian[I, J + 2] / sqrtMM;
                    Fm[I + 1, J] = Fm[J, I + 1] = hessian[I + 1, J] / sqrtMM;
                    Fm[I + 1, J + 1] = Fm[J + 1, I + 1] = hessian[I + 1, J + 1] / sqrtMM;
                    Fm[I + 1, J + 2] = Fm[J + 2, I + 1] = hessian[I + 1, J + 2] / sqrtMM;
                    Fm[I + 2, J] = Fm[J, I + 2] = hessian[I + 2, J] / sqrtMM;
                    Fm[I + 2, J + 1] = Fm[J + 1, I + 2] = hessian[I + 2, J + 1] / sqrtMM;
                    Fm[I + 2, J + 2] = Fm[J + 2, I + 2] = hessian[I + 2, J + 2] / sqrtMM;
                }
            }

            return;
        }

        /// <summary>
        /// 获取质心为原点的坐标
        /// </summary>
        /// <param name="coordinateOfMassCenter">质心为原点的笛卡尔坐标</param>
        public void GetCoordinateOfMassCenter(out BnulkMatrix coordinateOfMassCenter)
        {
            coordinateOfMassCenter = new BnulkMatrix(N, 3);
            //获取质心
            double[] tmpA = new double[3] { 0, 0, 0 };
            double tmpB = 0.0;
            double[] massCenter = new double[3] { 0, 0, 0 };
            for (int i = 0; i < N; i++)
            {
                tmpA[0] += realAtomicWeights[i] * x[i, 0];
                tmpA[1] += realAtomicWeights[i] * x[i, 1];
                tmpA[2] += realAtomicWeights[i] * x[i, 2];
                tmpB += realAtomicWeights[i];
            }
            massCenter[0] = tmpA[0] / tmpB;
            massCenter[1] = tmpA[1] / tmpB;
            massCenter[2] = tmpA[2] / tmpB;

            //把坐标原点移动到质心
            for (int i = 0; i < N; i++)
            {
                coordinateOfMassCenter[i, 0] = x[i, 0] - massCenter[0];
                coordinateOfMassCenter[i, 1] = x[i, 1] - massCenter[1];
                coordinateOfMassCenter[i, 2] = x[i, 2] - massCenter[2];
            }

            return;
        }

        /// <summary>
        /// 获得主惯性矩本征值及其向量
        /// </summary>
        /// <param name="coordinateOfMassCenter">质心为原点的笛卡尔坐标</param>
        /// <param name="principalMoments">主惯性矩本征值</param>
        /// <param name="X">主惯性矩本征向量数组</param>
        public void GetPrincipalMomentsAndEigenVec(out BnulkVec principalMoments, out BnulkMatrix X)
        {
            //计算转动惯量张量
            double mi = 0.0;
            principalMoments = new BnulkVec(3);
            X = new BnulkMatrix(3, 3);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    X.data[i, j] = 0.0;
                }
            }

            for (int i = 0; i < N; i++)
            {
                mi = realAtomicWeights[i];
                X.data[0, 0] += mi * (coordinateOfMassCenter[i, 1] * coordinateOfMassCenter[i, 1] + coordinateOfMassCenter[i, 2] * coordinateOfMassCenter[i, 2]);
                X.data[1, 1] += mi * (coordinateOfMassCenter[i, 0] * coordinateOfMassCenter[i, 0] + coordinateOfMassCenter[i, 2] * coordinateOfMassCenter[i, 2]);
                X.data[2, 2] += mi * (coordinateOfMassCenter[i, 0] * coordinateOfMassCenter[i, 0] + coordinateOfMassCenter[i, 1] * coordinateOfMassCenter[i, 1]);
                X.data[0, 1] -= mi * (coordinateOfMassCenter[i, 0] * coordinateOfMassCenter[i, 1]);
                X.data[0, 2] -= mi * (coordinateOfMassCenter[i, 0] * coordinateOfMassCenter[i, 2]);
                X.data[1, 2] -= mi * (coordinateOfMassCenter[i, 1] * coordinateOfMassCenter[i, 2]);
            }
            X.data[1, 0] = X.data[0, 1];
            X.data[2, 0] = X.data[0, 2];
            X.data[2, 1] = X.data[1, 2];

            //对角化
            BnulkVec e = new BnulkVec(3);
            BnulkMatrix.tred2(ref X, ref principalMoments, ref e);
            BnulkMatrix.tqli(ref principalMoments, ref e, ref X);

            return;
        }

        /// <summary>
        /// 获取振动的个数
        /// </summary>
        /// <param name="numberOfVibration">分子振动个数</param>
        public void GetNumberOfVibration(out int numberOfVibration)
        {
            switch (N)
            {
                case 1:                                                                    //单原子分子
                    numberOfVibration = 0;
                    break;
                case 2:
                    numberOfVibration = 0;                                                 //双原子分子
                    break;
                default:
                    BnulkVec tmpVec = NumericalRecipes.Sorting.Piksrt(principalMoments);
                    if (tmpVec[0] < 1E-4)                                                  //线性分子
                    {
                        numberOfVibration = 3 * N - 6;
                    }
                    else                                                                   //非线性分子
                    {
                        numberOfVibration = 3 * N - 7;
                    }
                    break;
            }
            return;
        }

        /// <summary>
        /// 获取3N行7列的平动转动阵
        /// </summary>
        /// <param name="D7">3N行7列的平动转动约束阵</param>
        public void GetD7(out BnulkMatrix D7)
        {
            int dim = 3 * N;
            D7 = new BnulkMatrix(dim, 7);

            BnulkMatrix P = new BnulkMatrix(dim, 3);
            P = coordinateOfMassCenter * X;

            int flag = 0;
            for (int i = 0; i < N; i++)
            {
                //平动
                flag = 3 * i;
                D7[flag, 0] = Math.Sqrt(realAtomicWeights[i]);
                D7[flag + 1, 0] = 0.0;
                D7[flag + 2, 0] = 0.0;
                D7[flag, 1] = 0.0;
                D7[flag + 1, 1] = Math.Sqrt(realAtomicWeights[i]);
                D7[flag + 2, 1] = 0.0;
                D7[flag, 2] = 0.0;
                D7[flag + 1, 2] = 0.0;
                D7[flag + 2, 2] = Math.Sqrt(realAtomicWeights[i]);

                //转动
                for (int j = 0; j < 3; j++)
                {
                    D7[flag + j, 3] = (P[i, 1] * X[j, 2] - P[i, 2] * X[j, 1]) / Math.Sqrt(realAtomicWeights[i]);
                    D7[flag + j, 4] = (P[i, 2] * X[j, 0] - P[i, 0] * X[j, 2]) / Math.Sqrt(realAtomicWeights[i]);
                    D7[flag + j, 5] = (P[i, 0] * X[j, 1] - P[i, 1] * X[j, 0]) / Math.Sqrt(realAtomicWeights[i]);
                }
            }

            //约束方向
            for(int i=0;i<dim;i++)
            {
                D7[i, 6] = gradient1[i] - gradient2[i];
            }

            //规范化平动、转动和约束方向。
            double length = 0.0;
            for (int i = 0; i < 7; i++)          //列
            {
                length = 0.0;
                for (int j = 0; j < dim; j++)        //行
                {
                    length += D7[j, i] * D7[j, i];
                }
                length = Math.Sqrt(length);

                for (int j = 0; j < dim; j++)
                {
                    if (length < 1E-12)
                    {
                        D7[j, i] = 0.0;
                    }
                    else
                    {
                        D7[j, i] = D7[j, i] / length;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// 检查D矩阵构造是否自洽
        /// </summary>
        /// <param name="D7">3N行7列的平动转动约束阵</param>
        /// <param name="complementarySetOfD">D阵的互补阵</param>
        /// <returns>D矩阵构造是否自洽</returns>
        public bool CheckComplementarySetOfD(BnulkMatrix D7, out BnulkMatrix complementarySetOfD)
        {
            bool check = true;
            int dim = 3 * N;
            int dimTR = dim - numberOfVibration;
            complementarySetOfD = new BnulkMatrix(dim, dimTR);
            double value = 0.0;
            int flag = 0;
            List<double[]> vecs = new List<double[]>(7);
            BnulkVec vec1 = new BnulkVec(dim);
            BnulkVec vec2 = new BnulkVec(dim);

            for (int i = 0; i < 7; i++)                             //列
            {
                for (int j = 0; j < dim; j++)                       //行
                {
                    vec1[j] = vec2[j] = D7[j, i];
                }
                value = vec1 | vec2;
                if (value < 1E-4)
                {
                    flag++;
                }
                else
                {
                    vecs.Add(BnulkVec.ToArray(vec1));
                }
            }

            if (dimTR != 7 - flag)
            {
                check = false;
            }
            for (int i = 0; i < dimTR; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    complementarySetOfD[j, i] = vecs[i][j];
                }
            }

            return check;
        }

        /// <summary>
        /// Sayvetz条件，把平动转动和约束从既约振动中分离出来的方案
        /// </summary>
        /// <param name="fullD">包含平动转动和约束的D矩阵</param>
        public void GetSayvetzMatrix(out BnulkMatrix fullD)
        {
            int dim = 3 * N;
            fullD = new BnulkMatrix(dim, dim);
            D = new BnulkMatrix(dim, numberOfVibration);
            BnulkMatrix D7;
            BnulkMatrix complementarySetOfD;
            bool selfConsistent = false;
            int shift = dim - numberOfVibration;

            GetD7(out D7);
            Output.WriteOutput.ShowMatrix(D7, "D7");
            selfConsistent = CheckComplementarySetOfD(D7, out complementarySetOfD);
            Output.WriteOutput.ShowMatrix(complementarySetOfD, "complementarySetOfD");

            if (selfConsistent == true)
            {
                //QR分解得到正交矩阵D
                BnulkMatrix Q = new BnulkMatrix(dim, dim);
                BnulkMatrix R = new BnulkMatrix(dim, dim);

                BnulkMatrix.qrdcmp(complementarySetOfD, out Q, out R);

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        fullD[i, j] = Q[i, j];
                    }
                }
            }
            else
            {
                Output.WriteOutput.Error.Append("ChemGo.HarmonicVibrationalAnalysis.GetSayvetzMatrix() Error:" + "\n" + "selfConsistent=false" + "\n");
                Console.WriteLine("ChemGo.HarmonicVibrationalAnalysis.GetSayvetzMatrix() Error:" + "\n" + "selfConsistent=false" + "\n");
                return;
            }
            return;
        }

        /// <summary>
        /// 获取内坐标系下的力常数矩阵
        /// </summary>
        /// <param name="Fint">既约振动力常数矩阵</param>
        /// <param name="fullFint">内坐标系下的完全力常数矩阵</param>
        public void GetFullFint(out BnulkMatrix Fint, out BnulkMatrix fullFint)
        {
            int dim = 3 * N;
            int shift = dim - numberOfVibration;
            fullFint = new BnulkMatrix(dim, dim);
            Fint = new BnulkMatrix(numberOfVibration, numberOfVibration);
            BnulkMatrix TransFullD = new BnulkMatrix(dim, dim);

            TransFullD = BnulkMatrix.Transpose(fullD);
            fullFint = TransFullD * Fm * fullD;

            for (int i = shift; i < dim; i++)
            {
                for (int j = shift; j < dim; j++)
                {
                    Fint[i - shift, j - shift] = fullFint[i, j];
                }
            }

            return;
        }

        /// <summary>
        /// 对角化内坐标下全部的力常数矩阵
        /// </summary>
        /// <param name="fullLambda">内坐标下完全力常数矩阵本征值</param>
        /// <param name="fullL">内坐标下力常数矩阵本征向量。即原子位移L矩阵</param>
        public void DiagFullFint(out BnulkVec fullEigenLambda, out BnulkMatrix fullL)
        {
            int dim = 3 * N;
            fullEigenLambda = new BnulkVec(dim);
            fullL = new BnulkMatrix(dim, dim);
            BnulkVec e = new BnulkVec(dim);

            fullL = BnulkMatrix.Copy(fullFint);

            BnulkMatrix.tred2(ref fullL, ref fullEigenLambda, ref e);
            BnulkMatrix.tqli(ref fullEigenLambda, ref e, ref fullL);

            return;
        }

        /// <summary>
        /// 把力常数本征值，转换为波数单位的频率
        /// </summary>
        /// <param name="vibrationalFrequencies">振动频率的本征值向量</param>
        public void CalVibrationalFrequencies(out BnulkVec fullFrequencies)
        {
            int dim = fullEigenLambda.dim;
            fullFrequencies = new BnulkVec(dim);

            for (int i = 0; i < dim; i++)
            {
                fullFrequencies[i] = fullEigenLambda[i] * PhysConst.hartree2J / PhysConst.bohr2m / PhysConst.bohr2m / PhysConst.amu2kg;
                if (fullFrequencies[i] >= 0)
                {
                    fullFrequencies[i] = Math.Sqrt(fullFrequencies[i]) / 2 / Math.PI;
                }
                else
                {
                    fullFrequencies[i] = (-1) * Math.Sqrt(-fullFrequencies[i]) / 2 / Math.PI;
                }
                fullFrequencies[i] = fullFrequencies[i] / PhysConst.c / 100;                                                                   //常用单位 cm-1
            }
            return;
        }

        /// <summary>
        /// 获取笛卡尔坐标下的全部平转振动向量
        /// </summary>
        /// <param name="fullNormalizationFactor">归一化因子</param>
        /// <param name="fullLCartesian">笛卡尔坐标下的平转振动向量</param>
        public void GetCartesianFullL(out BnulkVec fullNormalizationFactor, out BnulkMatrix fullLCartesian)
        {
            int dim = 3 * N;
            fullNormalizationFactor = new BnulkVec(dim);
            fullLCartesian = new BnulkMatrix(dim, dim);
            BnulkMatrix M = new BnulkMatrix(dim, dim);

            double sqrtM = 0.0;
            for (int i = 0; i < N; i++)
            {
                sqrtM = Math.Sqrt(realAtomicWeights[i]);
                M[3 * i, 3 * i] = 1 / sqrtM;
                M[3 * i + 1, 3 * i + 1] = 1 / sqrtM;
                M[3 * i + 2, 3 * i + 2] = 1 / sqrtM;
                M[3 * i, 3 * i + 1] = M[3 * i, 3 * i + 2] = M[3 * i + 1, 3 * i] = M[3 * i + 1, 3 * i + 2] = M[3 * i + 2, 3 * i] = M[3 * i + 2, 3 * i + 1] = 0.0;
            }

            fullLCartesian = M * fullD * fullL;

            for (int i = 0; i < dim; i++)             //列
            {

                fullNormalizationFactor[i] = 0.0;
                for (int j = 0; j < dim; j++)                       //行
                {
                    fullNormalizationFactor[i] += fullLCartesian[j, i] * fullLCartesian[j, i];
                }
                fullNormalizationFactor[i] = Math.Sqrt(1 / fullNormalizationFactor[i]);

                for (int j = 0; j < dim; j++)
                {
                    fullLCartesian[j, i] = fullLCartesian[j, i] * fullNormalizationFactor[i];
                }
            }

            return;
        }

        /// <summary>
        /// 从平转振动模式转换到振动模式
        /// </summary>
        /// <param name="vibrationalFrequencies">振动频率</param>
        /// <param name="vibrationalMode">振动模式</param>
        public void ToVibrationalData(out BnulkVec vibrationalFrequencies, out BnulkMatrix vibrationalMode)
        {
            int shift = fullFrequencies.dim - numberOfVibration;
            vibrationalFrequencies = new BnulkVec(numberOfVibration);
            vibrationalMode = new BnulkMatrix(fullFrequencies.dim, numberOfVibration);

            for (int i = 0; i < numberOfVibration; i++)             //列
            {
                vibrationalFrequencies[i] = fullFrequencies[i + shift];
                for (int j = 0; j < fullFrequencies.dim; j++)
                {
                    vibrationalMode[j, i] = fullMode[j, i + shift];
                }
            }

            return;
        }


        /// <summary>
        /// 按振动频率的大小排序
        /// </summary>
        /// <param name="vibrationalFrequencies">振动频率组成的向量</param>
        /// <param name="vibrationalMode">振动向量组成的矩阵</param>
        public void SortingHVA(ref BnulkVec vibrationalFrequencies, ref BnulkMatrix vibrationalMode)
        {
            //排序
            int dim = vibrationalFrequencies.dim;
            int row = vibrationalMode.row;
            BnulkVec eigenVecLable = new BnulkVec(dim);                                                 //标号数组，排序用
            for (int i = 0; i < dim; i++)                                                            //列代号
            {
                eigenVecLable[i] = i;
            }
            ChemKun.NumericalRecipes.Sorting.sort2(ref vibrationalFrequencies, ref eigenVecLable);

            //定义矢量数组，存储每列
            BnulkVec[] tmpVibrationalMode = new LinearAlgebra.BnulkVec[dim];
            for (int i = 0; i < dim; i++)
            {
                tmpVibrationalMode[i] = new BnulkVec(row);
            }
            for (int i = 0; i < dim; i++)                      //列
            {
                for (int j = 0; j < row; j++)                  //行
                {
                    tmpVibrationalMode[i].ele[j] = vibrationalMode[j, i];
                }
            }

            for (int i = 0; i < dim; i++)                                            //列
            {
                for (int j = 0; j < row; j++)                                        //行
                {
                    vibrationalMode[j, i] = tmpVibrationalMode[Convert.ToInt32(eigenVecLable[i])].ele[j];
                }
            }

            return;
        }

        /// <summary>
        /// 根据已经排好序的既约振动频率，判断是否为真正极小势能面交叉点
        /// </summary>
        /// <param name="vibrationalFrequencies">既约振动频率</param>
        /// <returns>是否真正极小势能面交叉点</returns>
        public bool IsRealMECP(BnulkVec vibrationalFrequencies)
        {
            bool isRealMECP = true;
            if (vibrationalFrequencies[0] < 0.0)
            {
                isRealMECP = false;
            }
            return isRealMECP;
        }

        /// <summary>
        /// 更新LiuFreq中的数据
        /// </summary>
        public void Update(ref Data_MECP.LiuFreq liuFreq)
        {
            liuFreq.isRealMECP = this.isRealMECP;
            return;
        }



        //显示计算初始值
        public void ShowInitialValue()
        {
            WriteOutput.ShowVector(gradient, "Lagrange Gradient");
            WriteOutput.ShowMatrix(hessian, "Lagrange Hessian");
            return;
        }
    }
}
