using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.Gaussian;
using ChemKun.Estimate;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void ObtainCalculatingData(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            switch (data_Input.kunData.calProgram)
            {
                case "gaussian":
                    ObtainCalculatingData_Gaussian(data_Input, ref data_MECP);
                    break;
                default:
                    break;
            }
            return;
        }

        private void ObtainCalculatingData_Gaussian(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            ReadGaussianOut readGaussianOut;
            int N;
            string[,] tmpParams;
            string[,] tmpHessian;
            double[,] tmpHessianDouble;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////                       读第一个态                              /////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (OS.OS.osClass == "windows")
            {
                readGaussianOut = new ReadGaussianOut("tmp\\State1_" + data_MECP.I.ToString() + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State1_" + data_MECP.I.ToString() + ".out");
            }
            
            //检验N值
            N = readGaussianOut.N;
            if (data_Input.gaussianInputSegment.N!=N)
            {
                Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
            }

            //获取能量
            switch (data_Input.mecpData.scfTyp1)
            {
                case "hftyp":
                    data_MECP.functionData.energy1 = readGaussianOut.GetHFEnergy();
                    break;
                case "cistyp":
                    data_MECP.functionData.energy1 = readGaussianOut.GetCISEnergy();
                    break;
                case "tdtyp":
                    data_MECP.functionData.energy1 = readGaussianOut.GetTDEnergy();
                    break;
                case "tdatyp":
                    data_MECP.functionData.energy1 = readGaussianOut.GetTDAEnergy();
                    break;
                default:
                    Output.WriteOutput.m_Result.Append("Get Energy Error.");
                    break;
            }
            

            switch (data_Input.gaussianInputSegment.coordinateType)                           //根据坐标类型，初始化参数
            {
                case "z-matrix":
                    //创建变量数组
                    tmpParams = new string[3 * N - 6, 3];
                    tmpHessian = new string[3 * N - 6, 3 * N - 6];
                    tmpHessianDouble = new double[3 * N - 6, 3 * N - 6];
                    //仅仅为了初始化赋值。因为这个数组在条件语句中赋值，所以需要初始化。
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            tmpHessianDouble[i, j] = -1;
                        }
                    }

                    //初始化data_MECP中的参数
                    data_MECP.functionData.para = new string[3 * N - 6];
                    data_MECP.functionData.x = new double[3 * N - 6];
                    data_MECP.functionData.gradient1 = new double[3 * N - 6];
                    data_MECP.functionData.hessian1 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.functionData.gradient2 = new double[3 * N - 6];
                    data_MECP.functionData.hessian2 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.newX = new double[3 * N - 6];

                    //读坐标和梯度
                    tmpParams = readGaussianOut.GetForceParams_Zmatrix();
                    //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        data_MECP.functionData.para[i] = tmpParams[i, 0];
                        data_MECP.functionData.x[i] = Convert.ToDouble(tmpParams[i, 1]);
                        data_MECP.functionData.gradient1[i] = Convert.ToDouble(tmpParams[i, 2]);
                    }

                    //读Hessian
                    //为了估算Hessian阵，存储上一步的梯度和坐标，定义相应矩阵 
                    data_MECP.estimateHessian.dim = 3 * N - 6;
                    data_MECP.estimateHessian.lastMatrixH1 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixH2 = new double[3 *N - 6, 3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixG1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixG2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixX1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixX2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixG1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixG2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixX1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixX2 = new double[3 * N - 6];
                    if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                    {
                        tmpHessian = readGaussianOut.GetForceConstant_Zmatrix();
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            for (int j = 0; j < 3 * N - 6; j++)
                            {
                                tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                            }
                        }  
                    }
                    else                                                                     //估算Hessian矩阵
                    {
                        for (int i = 0; i < data_MECP.estimateHessian.dim; i++)
                        {
                            data_MECP.estimateHessian.matrixX1[i] = data_MECP.functionData.x[i];
                            data_MECP.estimateHessian.matrixG1[i] = data_MECP.functionData.gradient1[i];
                        }
                        //获取上一步的坐标，梯度和Hessian
                        int Count = data_MECP.historyFunctionData.Count;
                        for(int i=0;i< 3 * N - 6;i++)
                        {
                            data_MECP.estimateHessian.lastMatrixX1[i] = data_MECP.historyFunctionData[Count - 1].x[i];
                            data_MECP.estimateHessian.lastMatrixG1[i] = data_MECP.historyFunctionData[Count - 1].gradient1[i];
                        }
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            for (int j = 0; j < 3 * N - 6; j++)
                            {
                                data_MECP.estimateHessian.lastMatrixH1[i, j] = data_MECP.historyFunctionData[Count - 1].hessian1[i, j];
                            }
                        }

                        EstimateHessian estimateHessian1 = new EstimateHessian(data_MECP.estimateHessian.dim, data_MECP.estimateHessian.lastMatrixX1, data_MECP.estimateHessian.lastMatrixG1,
                            data_MECP.estimateHessian.lastMatrixH1, data_MECP.estimateHessian.matrixX1, data_MECP.estimateHessian.matrixG1);

                        if (data_Input.mecpData.guessHessian == "Powell".ToLower())
                        {
                            tmpHessianDouble = estimateHessian1.Powell();
                        }
                        else
                        {
                            tmpHessianDouble = estimateHessian1.BFGS();
                        }
                    }

                    //根据计算结果，更新data_MECP.functionData中的Hessian阵
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            data_MECP.functionData.hessian1[i, j] = tmpHessianDouble[i, j];
                        }
                    }
                    data_MECP.functionData.N = N;
                    break;
                case "cartesian":
                    //创建变量数组
                    tmpParams = new string[3 * N, 3];
                    tmpHessian = new string[3 * N, 3 * N];
                    tmpHessianDouble = new double[3 * N, 3 * N];
                    //仅仅为了初始化赋值。因为这个数组在条件语句中赋值，所以需要初始化。
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            tmpHessianDouble[i, j] = -1;
                        }
                    }

                    //初始化data_MECP中的参数
                    data_MECP.functionData.para = new string[3 * N];
                    data_MECP.functionData.x = new double[3 * N];
                    data_MECP.functionData.gradient1 = new double[3 * N];
                    data_MECP.functionData.hessian1 = new double[3 * N, 3 * N];
                    data_MECP.functionData.gradient2 = new double[3 * N];
                    data_MECP.functionData.hessian2 = new double[3 * N, 3 * N];
                    data_MECP.newX = new double[3 * N];

                    //读坐标和梯度
                    tmpParams = readGaussianOut.GetForceParams_Cartesian();
                    //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                    for (int i = 0; i < 3 * N; i++)
                    {
                        data_MECP.functionData.para[i] = tmpParams[i, 0];
                        data_MECP.functionData.x[i] = Convert.ToDouble(tmpParams[i, 1]);
                        data_MECP.functionData.gradient1[i] = Convert.ToDouble(tmpParams[i, 2]);
                    }

                    //读Hessian
                    //为了估算Hessian阵，存储上一步的梯度和坐标，定义相应矩阵
                    //为了估算Hessian阵，存储上一步的梯度和坐标，定义相应矩阵 
                    data_MECP.estimateHessian.dim = 3 * N;
                    data_MECP.estimateHessian.lastMatrixH1 = new double[3 * N, 3 * N];
                    data_MECP.estimateHessian.lastMatrixH2 = new double[3 * N, 3 * N];
                    data_MECP.estimateHessian.lastMatrixG1 = new double[3 * N];
                    data_MECP.estimateHessian.lastMatrixG2 = new double[3 * N];
                    data_MECP.estimateHessian.lastMatrixX1 = new double[3 * N];
                    data_MECP.estimateHessian.lastMatrixX2 = new double[3 * N];
                    data_MECP.estimateHessian.matrixG1 = new double[3 * N];
                    data_MECP.estimateHessian.matrixG2 = new double[3 * N];
                    data_MECP.estimateHessian.matrixX1 = new double[3 * N];
                    data_MECP.estimateHessian.matrixX2 = new double[3 * N];
                    if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                    {
                        tmpHessian = readGaussianOut.GetForceConstant_Cartesian();
                        for (int i = 0; i < 3 * N; i++)
                        {
                            for (int j = 0; j < 3 * N; j++)
                            {
                                tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                            }
                        }
                    }
                    else                                                                     //估算Hessian矩阵
                    {
                        for (int i = 0; i < data_MECP.estimateHessian.dim; i++)
                        {
                            data_MECP.estimateHessian.matrixX1[i] = data_MECP.functionData.x[i];
                            data_MECP.estimateHessian.matrixG1[i] = data_MECP.functionData.gradient1[i];
                        }
                        //获取上一步的坐标，梯度和Hessian
                        int Count = data_MECP.historyFunctionData.Count;
                        for (int i = 0; i < 3 * N; i++)
                        {
                            data_MECP.estimateHessian.lastMatrixX1[i] = data_MECP.historyFunctionData[Count - 1].x[i];
                            data_MECP.estimateHessian.lastMatrixG1[i] = data_MECP.historyFunctionData[Count - 1].gradient1[i];
                        }
                        for (int i = 0; i < 3 * N; i++)
                        {
                            for (int j = 0; j < 3 * N; j++)
                            {
                                data_MECP.estimateHessian.lastMatrixH1[i, j] = data_MECP.historyFunctionData[Count - 1].hessian1[i, j];
                            }
                        }

                        EstimateHessian estimateHessian1 = new EstimateHessian(data_MECP.estimateHessian.dim, data_MECP.estimateHessian.lastMatrixX1, data_MECP.estimateHessian.lastMatrixG1,
                            data_MECP.estimateHessian.lastMatrixH1, data_MECP.estimateHessian.matrixX1, data_MECP.estimateHessian.matrixG1);

                        if (data_Input.mecpData.guessHessian == "Powell".ToLower())
                        {
                            tmpHessianDouble = estimateHessian1.Powell();
                        }
                        else
                        {
                            tmpHessianDouble = estimateHessian1.BFGS();
                        }
                    }


                    //根据计算结果，更新data_MECP.functionData中的Hessian阵
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            data_MECP.functionData.hessian1[i, j] = tmpHessianDouble[i, j];
                        }
                    }
                    data_MECP.functionData.N = N;
                    break;
                default:
                    break;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////                       读第二个态                              /////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (OS.OS.osClass == "windows")
            {
                readGaussianOut = new ReadGaussianOut("tmp\\State2_" + data_MECP.I.ToString() + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State2_" + data_MECP.I.ToString() + ".out");
            }
            
            //检验N值
            N = readGaussianOut.N;
            if (data_Input.gaussianInputSegment.N != N)
            {
                Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
            }

            //获取能量
            switch (data_Input.mecpData.scfTyp2)
            {
                case "hftyp":
                    data_MECP.functionData.energy2 = readGaussianOut.GetHFEnergy(); ;
                    break;
                case "cistyp":
                    data_MECP.functionData.energy2 = readGaussianOut.GetCISEnergy(); ;
                    break;
                case "tdtyp":
                    data_MECP.functionData.energy2 = readGaussianOut.GetTDEnergy();
                    break;
                case "tdatyp":
                    data_MECP.functionData.energy2 = readGaussianOut.GetTDAEnergy();
                    break;
                default:
                    Output.WriteOutput.m_Result.Append("Get Energy Error.");
                    break;
            }

            switch (data_Input.gaussianInputSegment.coordinateType)                           //根据坐标类型，初始化参数
            {
                case "z-matrix":
                    //创建变量数组
                    tmpParams = new string[3 * N - 6, 3];
                    tmpHessian = new string[3 * N - 6, 3 * N - 6];
                    tmpHessianDouble = new double[3 * N - 6, 3 * N - 6];
                    //仅仅为了初始化赋值。因为这个数组在条件语句中赋值，所以需要初始化。
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            tmpHessianDouble[i, j] = -1;
                        }
                    }
                    //读坐标和梯度
                    tmpParams = readGaussianOut.GetForceParams_Zmatrix();
                    //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        data_MECP.functionData.para[i] = tmpParams[i, 0];
                        data_MECP.functionData.x[i] = Convert.ToDouble(tmpParams[i, 1]);
                        data_MECP.functionData.gradient2[i] = Convert.ToDouble(tmpParams[i, 2]);
                    }

                    //读Hessian
                    /*  第一个态已经创建过的对象。
                    //为了估算Hessian阵，存储上一步的梯度和坐标，定义相应矩阵 
                    data_MECP.estimateHessian.dim = 3 * N - 6;
                    data_MECP.estimateHessian.lastMatrixH1 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixH2 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixG1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixG2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixX1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.lastMatrixX2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixG1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixG2 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixX1 = new double[3 * N - 6];
                    data_MECP.estimateHessian.matrixX2 = new double[3 * N - 6];
                    */
                    if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                    {
                        tmpHessian = readGaussianOut.GetForceConstant_Zmatrix();
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            for (int j = 0; j < 3 * N - 6; j++)
                            {
                                tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                            }
                        }
                    }
                    else                                                                     //估算Hessian矩阵
                    {
                        for (int i = 0; i < data_MECP.estimateHessian.dim; i++)
                        {
                            data_MECP.estimateHessian.matrixX2[i] = data_MECP.functionData.x[i];
                            data_MECP.estimateHessian.matrixG2[i] = data_MECP.functionData.gradient2[i];
                        }
                        //获取上一步的坐标，梯度和Hessian
                        int Count = data_MECP.historyFunctionData.Count;
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            data_MECP.estimateHessian.lastMatrixX2[i] = data_MECP.historyFunctionData[Count - 1].x[i];
                            data_MECP.estimateHessian.lastMatrixG2[i] = data_MECP.historyFunctionData[Count - 1].gradient2[i];
                        }
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            for (int j = 0; j < 3 * N - 6; j++)
                            {
                                data_MECP.estimateHessian.lastMatrixH2[i, j] = data_MECP.historyFunctionData[Count - 1].hessian2[i, j];
                            }
                        }

                        EstimateHessian estimateHessian2 = new EstimateHessian(data_MECP.estimateHessian.dim, data_MECP.estimateHessian.lastMatrixX2, data_MECP.estimateHessian.lastMatrixG2,
                            data_MECP.estimateHessian.lastMatrixH2, data_MECP.estimateHessian.matrixX2, data_MECP.estimateHessian.matrixG2);

                        if (data_Input.mecpData.guessHessian == "Powell".ToLower())
                        {
                            tmpHessianDouble = estimateHessian2.Powell();
                        }
                        else
                        {
                            tmpHessianDouble = estimateHessian2.BFGS();
                        }
                    }
                    /* 已经在第一个态中初始化
                    //初始化data_MECP中的参数
                    data_MECP.functionData.para = new string[3 * N - 6];
                    data_MECP.functionData.x = new double[3 * N - 6];
                    data_MECP.functionData.gradient1 = new double[3 * N - 6];
                    data_MECP.functionData.hessian1 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.functionData.gradient2 = new double[3 * N - 6];
                    data_MECP.functionData.hessian2 = new double[3 * N - 6, 3 * N - 6];
                    data_MECP.newX = new double[3 * N - 6];
                    */
                    //根据计算结果，更新data_MECP.functionData中的Hessian阵
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            data_MECP.functionData.hessian2[i, j] = tmpHessianDouble[i, j];
                        }
                    }
                    data_MECP.functionData.N = N;
                    break;

                case "cartesian":
                    //创建变量数组
                    tmpParams = new string[3 * N, 3];
                    tmpHessian = new string[3 * N, 3 * N];
                    tmpHessianDouble = new double[3 * N, 3 * N];
                    //仅仅为了初始化赋值。因为这个数组在条件语句中赋值，所以需要初始化。
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            tmpHessianDouble[i, j] = -1;
                        }
                    }

                    //读坐标和梯度
                    tmpParams = readGaussianOut.GetForceParams_Cartesian();
                    //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                    for (int i = 0; i < 3 * N; i++)
                    {
                        data_MECP.functionData.para[i] = tmpParams[i, 0];
                        data_MECP.functionData.x[i] = Convert.ToDouble(tmpParams[i, 1]);
                        data_MECP.functionData.gradient2[i] = Convert.ToDouble(tmpParams[i, 2]);
                    }

                    //读Hessian
                    //为了估算Hessian阵，存储上一步的梯度和坐标，定义相应矩阵 
                    if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                    {
                        tmpHessian = readGaussianOut.GetForceConstant_Cartesian();
                        for (int i = 0; i < 3 * N; i++)
                        {
                            for (int j = 0; j < 3 * N; j++)
                            {
                                tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                            }
                        }
                    }
                    else                                                                     //估算Hessian矩阵
                    {
                        for (int i = 0; i < data_MECP.estimateHessian.dim; i++)
                        {
                            data_MECP.estimateHessian.matrixX2[i] = data_MECP.functionData.x[i];
                            data_MECP.estimateHessian.matrixG2[i] = data_MECP.functionData.gradient2[i];
                        }
                        //获取上一步的坐标，梯度和Hessian
                        int Count = data_MECP.historyFunctionData.Count;
                        for (int i = 0; i < 3 * N; i++)
                        {
                            data_MECP.estimateHessian.lastMatrixX2[i] = data_MECP.historyFunctionData[Count - 1].x[i];
                            data_MECP.estimateHessian.lastMatrixG2[i] = data_MECP.historyFunctionData[Count - 1].gradient2[i];
                        }
                        for (int i = 0; i < 3 * N; i++)
                        {
                            for (int j = 0; j < 3 * N; j++)
                            {
                                data_MECP.estimateHessian.lastMatrixH2[i, j] = data_MECP.historyFunctionData[Count - 1].hessian2[i, j];
                            }
                        }

                        EstimateHessian estimateHessian2 = new EstimateHessian(data_MECP.estimateHessian.dim, data_MECP.estimateHessian.lastMatrixX2, data_MECP.estimateHessian.lastMatrixG2,
                            data_MECP.estimateHessian.lastMatrixH2, data_MECP.estimateHessian.matrixX2, data_MECP.estimateHessian.matrixG2);

                        if (data_Input.mecpData.guessHessian == "Powell".ToLower())
                        {
                            tmpHessianDouble = estimateHessian2.Powell();
                        }
                        else
                        {
                            tmpHessianDouble = estimateHessian2.BFGS();
                        }
                    }

                    /*在第一个态中已经初始化过了。
                    //初始化data_MECP中的参数
                    data_MECP.functionData.para = new string[3 * N];
                    data_MECP.functionData.x = new double[3 * N];
                    data_MECP.functionData.gradient1 = new double[3 * N];
                    data_MECP.functionData.hessian1 = new double[3 * N, 3 * N];
                    data_MECP.functionData.gradient2 = new double[3 * N];
                    data_MECP.functionData.hessian2 = new double[3 * N, 3 * N];
                    data_MECP.newX = new double[3 * N];
                    */

                    //根据计算结果，更新data_MECP.functionData中的Hessian阵
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            data_MECP.functionData.hessian2[i, j] = tmpHessianDouble[i, j];
                        }
                    }
                    data_MECP.functionData.N = N;
                    break;
                default:
                    break;
            }

            data_MECP.functionData.I = data_MECP.I;
            //存储历史数据
            data_MECP.historyFunctionData.Add(data_MECP.functionData);


            /*
            //删除历史数据，防止占用内存过大。
            if(data_MECP.historyFunctionData.Count>=5)
            {
                data_MECP.historyFunctionData.RemoveAt(0);
            }
            */
            return;
        }
    }
}
