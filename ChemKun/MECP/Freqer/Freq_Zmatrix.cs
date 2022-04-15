using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.MECP;
using ChemKun.LinearAlgebra;

namespace ChemKun.MECP.Freqer
{
    class Freq_Zmatrix
    {
        public Freq_Zmatrix(Data_MECP.FunctionData functionData, ref Data_MECP.Freq freq)
        {
            //初始化
            freq.dim = 3 * functionData.N - 5;
            freq.oldBorderedHessian = new double[freq.dim, freq.dim];
            freq.borderedHessian = new double[freq.dim, freq.dim];
            freq.principalMinors = new double[freq.dim - 1];
            freq.diagonalBorderedHessian = new double[freq.dim];

            for (int i = 0; i < freq.dim; i++)
            {
                for (int j = 0; j < freq.dim; j++)
                {
                    freq.borderedHessian[i, j] = 0;
                }
            }

            /*
            freq.eigenVecBorderedHessian = new BnulkVec[freq.dim];            

            for (int i = 0; i < freq.dim; i++)
            {
                freq.eigenVecBorderedHessian[i] = new BnulkVec(freq.dim);
            }
            //
            */

            try
            {
                ///
                //构建bordered hessian矩阵
                ///

                freq.borderedHessian[0, 0] = 0;
                for (int i = 1; i < freq.dim; i++)
                {
                    freq.borderedHessian[0, i] = functionData.gradient1[i - 1] - functionData.gradient2[i - 1];
                    freq.borderedHessian[i, 0] = functionData.gradient1[i - 1] - functionData.gradient2[i - 1];
                }
                for (int i = 1; i < freq.dim; i++)
                {
                    for (int j = 1; j < freq.dim; j++)
                    {
                        freq.borderedHessian[i, j] = functionData.hessian1[i - 1, j - 1] - functionData.Lambda * (functionData.hessian1[i - 1, j - 1] - functionData.hessian2[i - 1, j - 1]);
                    }
                }

                for (int i = 0; i < freq.dim; i++)
                {
                    for (int j = 0; j < freq.dim; j++)
                    {
                        freq.oldBorderedHessian[i, j] = freq.borderedHessian[i, j];
                    }
                }

                /*
                //
                ///
                //按梯度由小到大重排bordered hessian矩阵
                ///
                //RearrangementBorderedHessian(freq.dim, ref freq.borderedHessian);

                ///
                //检测1 对角化判断正定矩阵
                ///

                DiagonalBorderedHessian(freq.borderedHessian, freq.dim, ref freq.diagonalBorderedHessian, ref freq.eigenVecBorderedHessian);

                ///
                //检测2 由左上逐次计算主子式的值
                ///

                CalPrincipalMinors(freq.borderedHessian, freq.dim, ref freq.principalMinors);
                */

                ///
                //检测3 切空间正定测试
                ///
                /*                test1 叶荫宇 P336
                double[] tmp1 = { 1, 1, 1 };
                freq.gradientConstrainedConditions = new BnulkVec(tmp1);
                double[,] tmp2 = { { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 0 } };
                freq.L = new BnulkMatrix(tmp2);
                */

                freq.gradientConstrainedConditions = new BnulkVec(freq.dim - 1);
                freq.L = new BnulkMatrix(freq.dim - 1, freq.dim - 1);
                for(int i=0;i<freq.dim-1;i++)
                {
                    freq.gradientConstrainedConditions[i] = freq.borderedHessian[0, i + 1];
                }
                for(int i=0;i<freq.dim-1;i++)
                {
                    for(int j=0;j<freq.dim-1;j++)
                    {
                        freq.L[i, j] = freq.borderedHessian[i + 1, j + 1];
                    }
                }
                TestTangentSubspace(freq.dim - 1, freq.gradientConstrainedConditions, freq.L, out freq.E, out freq.EtLE, out freq.eigenValueEtLE, out freq.eigenVecEtLE);
            }
            catch
            {
                Output.WriteOutput.m_Result.Append("ChemKun.MECP.Freqer Error" + "\n");
                Console.WriteLine("ChemKun.MECP.Freqer Error" + "\n");
            }
            return;
        }

        /// <summary>
        /// 对角化加边矩阵
        /// </summary>
        /// <param name="borderedHessian">加边矩阵</param>
        /// <param name="dim">加边矩阵的维数</param>
        /// <param name="diagonalBorderedHessian">对角元素</param>
        /// <param name="eigenVecBorderedHessian">本征向量</param>
        private void DiagonalBorderedHessian(double[,] borderedHessian, int dim, ref double[] diagonalBorderedHessian, ref BnulkVec[] eigenVecBorderedHessian)
        {
            BnulkMatrix forDiagonalBorderedHessian = new BnulkMatrix(dim, dim);
            BnulkVec d = new BnulkVec(dim);
            BnulkVec e = new BnulkVec(dim);

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    forDiagonalBorderedHessian[i, j] = borderedHessian[i, j];
                }
            }

            BnulkMatrix.tred2(ref forDiagonalBorderedHessian, ref d, ref e);
            BnulkMatrix.tqli(ref d, ref e, ref forDiagonalBorderedHessian);

            for (int i = 0; i < dim; i++)
            {
                diagonalBorderedHessian[i] = d[i];
            }
            for (int i = 0; i < dim; i++)
            {
                //把第i列传给向量数组
                for (int j = 0; j < dim; j++)
                {
                    eigenVecBorderedHessian[i].ele[j] = forDiagonalBorderedHessian[j, i];
                }
            }
            return;
        }

        /// <summary>
        /// 由左上逐次计算主子式的值
        /// </summary>
        /// <param name="borderedHessian">加边矩阵</param>
        /// <param name="dim">加边矩阵的维数</param>
        /// <param name="principalMinors">主子式</param>
        private void CalPrincipalMinors(double[,] borderedHessian, int dim, ref double[] principalMinors)
        {
            //先换第1,2列
            double[,] changeBorderedHessian = new double[dim, dim];
            double[] tmpColumn = new double[dim];
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    changeBorderedHessian[i, j] = borderedHessian[i, j];
                }
            }
            for (int i = 0; i < dim; i++)
            {
                tmpColumn[i] = changeBorderedHessian[i, 0];
                changeBorderedHessian[i, 0] = changeBorderedHessian[i, 1];
                changeBorderedHessian[i, 1] = tmpColumn[i];
            }

            //操作
            BnulkMatrix matrixA;                                                         //构造矩阵
            CalcDet calcDet = new CalcDet();                                             //计算行列式
            for (int check = 1; check < dim; check++)
            {
                double[,] a = new double[check + 1, check + 1];
                for (int i = 0; i < check + 1; i++)
                {
                    for (int j = 0; j < check + 1; j++)
                    {
                        a[i, j] = changeBorderedHessian[i, j];
                    }
                }
                matrixA = new BnulkMatrix(a);
                calcDet.det(matrixA, ref principalMinors[check - 1]);
                principalMinors[check - 1] = (-1) * principalMinors[check - 1];
                //freq.detDiagonal[check - 1] = MatrixSurplus(a);
            }

            return;
        }

        private void TestTangentSubspace(int dim, BnulkVec gradientConstrainedConditions, BnulkMatrix L, out BnulkMatrix E, out BnulkMatrix EtLE, out BnulkVec eigenvalues, out BnulkVec[] eigenvectors)
        {
            BnulkMatrix gAndE = new BnulkMatrix(dim, dim);
            E = new BnulkMatrix(dim, dim - 1);
            BnulkMatrix Et = new BnulkMatrix(dim - 1, dim);
            BnulkMatrix Q = new BnulkMatrix(dim, dim);
            BnulkMatrix R = new BnulkMatrix(dim, dim);
            EtLE = new BnulkMatrix(dim - 1, dim - 1);
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    gAndE[i, j] = 0.0;
                }
            }
            for (int i = 0; i < dim; i++)
            {
                gAndE[i, i] = 1.0;
                gAndE[i, 0] = gradientConstrainedConditions[i];
            }

            BnulkMatrix.qrdcmp(gAndE, out Q, out R);

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim - 1; j++)
                {
                    E[i, j] = Q[i, j + 1];
                }
            }

            /*
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    L[i, j] = borderedHessian[i + 1, j + 1];
                }
            }
            */

            Et = BnulkMatrix.Transpose(E);
            EtLE = Et * L * E;

            BnulkMatrix forEigenvectors = new BnulkMatrix(dim - 1, dim - 1);
            BnulkVec forEigenvalues = new BnulkVec(dim - 1);
            BnulkVec e = new BnulkVec(dim - 1);

            for (int i = 0; i < dim - 1; i++)
            {
                for (int j = 0; j < dim - 1; j++)
                {
                    forEigenvectors[i, j] = EtLE[i, j];
                }
            }

            BnulkMatrix.tred2(ref forEigenvectors, ref forEigenvalues, ref e);
            BnulkMatrix.tqli(ref forEigenvalues, ref e, ref forEigenvectors);

            eigenvalues = new BnulkVec(dim - 1);
            eigenvectors = new BnulkVec[dim - 1];
            for (int i = 0; i < dim-1; i++)
            {
                eigenvectors[i] = new BnulkVec(dim - 1);
            }

            for (int i = 0; i < dim-1; i++)
            {
                eigenvalues[i] = forEigenvalues[i];
            }
            for (int i = 0; i < dim-1; i++)
            {
                //把第i列传给向量数组
                for (int j = 0; j < dim-1; j++)
                {
                    eigenvectors[i].ele[j] = forEigenvectors[j, i];
                }
            }

            return;
        }

        /// <summary>
        /// 没啥用，本来想排序的。
        /// </summary>
        /// <param name="dim">维数</param>
        /// <param name="borderedHessian">加边矩阵</param>
        private void RearrangementBorderedHessian(int dim, ref double[,] borderedHessian)
        {
            int i, j, inc;
            double[,] result = new double[dim, dim];
            double[] v = new double[dim];           //行
            double[] w = new double[dim];           //列

            inc = 1;                                //确定初始量
            do
            {
                inc *= 3;
                inc++;
            } while (inc <= dim);

            do                                      //部分排序循环
            {
                inc /= 3;
                for (i = inc; i < dim; i++)                //直接插入的外循环
                {
                    for (int d = 0; d < dim; d++)
                    {
                        v[d] = borderedHessian[d, i];
                        w[d] = borderedHessian[i, d];
                    }
                    j = i;
                    while (borderedHessian[0, j - inc] > v[0])                     //直接插入的内循环
                    {
                        for (int d = 0; d < dim; d++)
                        {
                            borderedHessian[d, j] = borderedHessian[d, j - inc];
                        }
                        for (int d = 0; d < dim; d++)
                        {
                            borderedHessian[d, j] = borderedHessian[d, j - inc];
                        }
                        j -= inc;
                        if (j < inc)
                            break;
                    }
                    for (int d = 0; d < dim; d++)
                    {
                        borderedHessian[d, j] = v[d];
                    }
                    for (int d = 0; d < dim; d++)
                    {
                        borderedHessian[d, j] = v[d];
                    }
                }
            } while (inc > 1);

            return;
        }





    }
}
