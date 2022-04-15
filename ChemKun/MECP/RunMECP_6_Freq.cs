using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using ChemKun.Data;
using ChemKun.Gaussian;
using ChemKun.MECP.Freqer;
using ChemKun.LinearAlgebra;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void Freq(Data_Input data_Input, Data_MECP data_MECP, ref Data_MECP.Freq freq)
        {
            //开始
            if (data_MECP.functionData.coordinateType == "z-matrix" && data_MECP.mecpFreq =="simple")
            {
                //输出标志
                StringBuilder m_Result = new StringBuilder();
                m_Result.Append("\n");
                m_Result.Append(" **********************************************************************" + "\n" + "\n");
                m_Result.Append("             MECP Freq analysis using the simple method.");
                m_Result.Append(" **********************************************************************" + "\n" + "\n");

                if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                {
                    if (OS.OS.osClass == "windows")
                    {
                        File.Copy("tmp\\" + "State1_" + data_MECP.I.ToString() + ".out", "tmp\\" + "State1_Freq.out", true);
                        File.Copy("tmp\\" + "State2_" + data_MECP.I.ToString() + ".out", "tmp\\" + "State2_Freq.out", true);
                    }
                    else
                    {
                        File.Copy("tmp//" + "State1_" + data_MECP.I.ToString() + ".out", "tmp//" + "State1_Freq.out", true);
                        File.Copy("tmp//" + "State2_" + data_MECP.I.ToString() + ".out", "tmp//" + "State2_Freq.out", true);
                    }
                }
                else
                {
                    if (data_MECP.functionData.coordinateType == "z-matrix")
                    {
                        //创建Freq输入文件
                        switch (data_Input.kunData.calProgram)
                        {
                            case "gaussian":
                                CreateFreqFiles_Gaussian(data_Input.gaussianInputSegment, data_MECP);
                                break;
                            default:
                                break;
                        }
                        //计算单点
                        switch (data_Input.kunData.calProgram)
                        {
                            case "gaussian":
                                CalculateFreqSinglePoints_Gaussian(data_Input);
                                break;
                            default:
                                break;
                        }
                    }
                }
                //获取梯度和力常数
                switch (data_Input.kunData.calProgram)
                {
                    case "gaussian":
                        ObtainFreqCalculatingData_Gaussian_zMatrix(data_Input, ref data_MECP);
                        break;
                    default:
                        break;
                }
                //二阶充分条件验证
                Freq_Zmatrix freq_Zmatrix = new Freq_Zmatrix(data_MECP.functionData, ref data_MECP.freq);
                //排序
                int cycle = data_MECP.freq.dim - 2;
                LinearAlgebra.BnulkVec eigenVecLable = new LinearAlgebra.BnulkVec(cycle);
                for (int i = 0; i < cycle; i++)
                {
                    eigenVecLable[i] = i;
                }
                ChemKun.NumericalRecipes.Sorting.sort2(ref data_MECP.freq.eigenValueEtLE, ref eigenVecLable);

                LinearAlgebra.BnulkVec[] tmpEigenVecEtLE = new LinearAlgebra.BnulkVec[cycle];
                for (int i = 0; i < cycle; i++)
                {
                    tmpEigenVecEtLE[i] = new LinearAlgebra.BnulkVec(cycle);
                }
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 0; j < cycle; j++)
                    {
                        tmpEigenVecEtLE[i].ele[j] = data_MECP.freq.eigenVecEtLE[i].ele[j];
                    }
                }
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 0; j < cycle; j++)
                    {
                        data_MECP.freq.eigenVecEtLE[i].ele[j] = tmpEigenVecEtLE[Convert.ToInt32(eigenVecLable[i])].ele[j];
                    }
                }
                //验证收否为真正极小
                freq.isRealMECP = true;
                cycle = data_MECP.freq.eigenValueEtLE.dim;
                for (int i = 0; i < cycle; i++)
                {
                    if (data_MECP.freq.eigenValueEtLE[i] < 0.0)
                    {
                        freq.isRealMECP = false;
                    };
                }
                return;
            }

            else
            {
                //输出标志
                StringBuilder m_Result = new StringBuilder();
                m_Result.Append("\n");
                m_Result.Append(" **********************************************************************" + "\n" + "\n");
                m_Result.Append("              MECP Freq analysis using the Liu method.");
                m_Result.Append(" **********************************************************************" + "\n" + "\n");
                Output.WriteOutput.Write();


                if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                {
                    if (OS.OS.osClass == "windows")
                    {
                        File.Copy("tmp\\" + "State1_" + data_MECP.I.ToString() + ".out", "tmp\\" + "State1_Freq.out", true);
                        File.Copy("tmp\\" + "State2_" + data_MECP.I.ToString() + ".out", "tmp\\" + "State2_Freq.out", true);
                    }
                    else
                    {
                        File.Copy("tmp//" + "State1_" + data_MECP.I.ToString() + ".out", "tmp//" + "State1_Freq.out", true);
                        File.Copy("tmp//" + "State2_" + data_MECP.I.ToString() + ".out", "tmp//" + "State2_Freq.out", true);
                    }
                }
                else
                {
                    if (data_MECP.functionData.coordinateType == "z-matrix")
                    {
                        //创建Freq输入文件
                        switch (data_Input.kunData.calProgram)
                        {
                            case "gaussian":
                                CreateFreqFiles_Gaussian(data_Input.gaussianInputSegment, data_MECP);
                                break;
                            default:
                                break;
                        }
                        //计算单点
                        switch (data_Input.kunData.calProgram)
                        {
                            case "gaussian":
                                CalculateFreqSinglePoints_Gaussian(data_Input);
                                break;
                            default:
                                break;
                        }
                    }
                }

                //获取Lambda值
                data_MECP.liuFreq.Lambda = data_MECP.functionData.Lambda;
                //获取梯度和力常数
                switch (data_Input.kunData.calProgram)
                {
                    case "gaussian":
                        ObtainFreqCalculatingData_Gaussian(data_Input, ref data_MECP.liuFreq);
                        break;
                    default:
                        break;
                }

                //二阶充分条件验证
                ReducedHVA reducedHVA = new ReducedHVA(data_MECP.liuFreq);
                reducedHVA.Running();
                reducedHVA.Update(ref data_MECP.liuFreq);

            }

            return;
        }

        private void CreateFreqFiles_Gaussian(Data_Input.GaussianInputSegment gaussianInputSegment, Data_MECP data_MECP)
        {
            Data_Gjf.GjfSegment gjf1Segment, gjf2Segment;
            gjf1Segment.firstSection = new List<string>();
            gjf1Segment.routeSection = new List<string>();
            gjf1Segment.titleSection = new List<string>();
            gjf1Segment.chargeAndMultiplicity = null;
            gjf1Segment.coordinateType = gaussianInputSegment.coordinateType;
            gjf1Segment.molecularSpecification_ZMatrix = new List<string>();
            gjf1Segment.molecularPara_ZMatrix = new List<string>();
            gjf1Segment.molecularCartesian = new List<string>();
            gjf1Segment.addition = new List<string>();
            gjf2Segment.firstSection = new List<string>();
            gjf2Segment.routeSection = new List<string>();
            gjf2Segment.titleSection = new List<string>();
            gjf2Segment.chargeAndMultiplicity = null;
            gjf2Segment.coordinateType = gaussianInputSegment.coordinateType;
            gjf2Segment.molecularSpecification_ZMatrix = new List<string>();
            gjf2Segment.molecularPara_ZMatrix = new List<string>();
            gjf2Segment.molecularCartesian = new List<string>();
            gjf2Segment.addition = new List<string>();

            StreamWriter newGjf;                                             //用于产生Gjf文件

            MECP.InputFileConverter.ConvertGaussian.ToGjfSegment(gaussianInputSegment, ref gjf1Segment, ref gjf2Segment);

            switch (data_MECP.functionData.coordinateType)                           //根据坐标类型，初始化参数
            {
                case "z-matrix":
                    for (int i = 0; i < (data_MECP.functionData.N - 1); i++)     //原子参数（波尔）转为埃
                    {
                        data_MECP.functionData.x[i] = data_MECP.functionData.x[i] * 0.529177249;                //波尔转埃
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);     //保留小数点后6位
                    }

                    for (int i = data_MECP.functionData.N - 1; i < (3 * data_MECP.functionData.N - 6); i++) //原子参数（弧度）转为度
                    {
                        data_MECP.functionData.x[i] = data_MECP.functionData.x[i] * 180 / System.Math.PI;              //=180/3.1415927
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);            //保留小数点后6位
                    }
                    //新参数角度部分大于180或者小于0
                    for (int i = data_MECP.functionData.N - 1; i < (2 * data_MECP.functionData.N - 3); i++) //原子参数（弧度）转为度
                    {
                        if (data_MECP.functionData.x[i] > 180.0 || data_MECP.functionData.x[i] < 0.0)
                        {
                            Output.WriteOutput.m_Result.Append("Error. The new angle is greater than 180 degrees or less than 0 degrees." + "\n");
                            Console.WriteLine("Error. The new angle is greater than 180 degrees or less than 0 degrees." + "\n");
                        }

                    }
                    break;
                case "cartesian":
                    for (int i = 0; i < (3 * data_MECP.functionData.N); i++)     //原子参数（波尔）转为埃
                    {
                        data_MECP.functionData.x[i] = data_MECP.functionData.x[i] * 0.529177249;                //波尔转埃
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);     //保留小数点后6位
                    }
                    break;
                default:
                    break;
            }

            //整数问题
            string[] strX = new string[data_MECP.functionData.x.Length];                                  //字符串形式的x     
            for (int i = 0; i < data_MECP.functionData.x.Length; i++)
            {
                //如果Result[h]为整数，加上小数点
                if (Math.Floor(data_MECP.functionData.x[i]) == data_MECP.functionData.x[i])
                {
                    strX[i] = data_MECP.functionData.x[i].ToString() + ".0";
                }
                else
                {
                    strX[i] = data_MECP.functionData.x[i].ToString();
                }
            }


            //产生第一个态的Gjf文件
            if (OS.OS.osClass == "windows")
            {
                newGjf = File.CreateText("tmp\\" + "State1_Freq" + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State1_Freq" + ".gjf");
            }

            for (int i = 0; i < gjf1Segment.firstSection.Count; i++)
            {
                newGjf.Write(gjf1Segment.firstSection[i] + "\n");
            }
            for (int i = 0; i < gjf1Segment.routeSection.Count; i++)
            {
                if (i == gjf1Segment.routeSection.Count - 1)
                {
                    newGjf.Write(gjf1Segment.routeSection[i] + " freq=noraman IOP(7/33=1)" + "\n");
                }
                else
                {
                    newGjf.Write(gjf1Segment.routeSection[i] + "\n");
                }
            }
            newGjf.Write("\n");
            for (int i = 0; i < gjf1Segment.titleSection.Count; i++)
            {
                newGjf.Write(gjf1Segment.titleSection[i] + "\n");
            }
            newGjf.Write("\n");
            newGjf.Write(gjf1Segment.chargeAndMultiplicity + "\n");
            //坐标
            if (gjf1Segment.coordinateType == "z-matrix")
            {
                for (int i = 0; i < gjf1Segment.molecularSpecification_ZMatrix.Count; i++)
                {
                    newGjf.Write(gjf1Segment.molecularSpecification_ZMatrix[i] + "\n");
                }
                newGjf.Write("\n");
                for (int i = 0; i < gjf1Segment.molecularPara_ZMatrix.Count; i++)
                {
                    newGjf.Write(data_MECP.functionData.para[i].PadRight(10) + "=     " + strX[i] + "\n");
                }
            }
            if (gjf1Segment.coordinateType == "cartesian")
            {
                for (int i = 0; i < gjf1Segment.molecularCartesian.Count; i++)
                {
                    newGjf.Write(gjf1Segment.molecularCartesian[i].Substring(0, 3).PadRight(10) + strX[3 * i].PadRight(15) + strX[3 * i + 1].PadRight(10) + strX[3 * i + 2].PadRight(10) + "\n");
                }
            }
            //附加部分
            newGjf.Write("\n");
            for (int i = 0; i < gjf1Segment.addition.Count; i++)
            {
                newGjf.Write(gjf1Segment.addition[i] + "\n");
            }
            newGjf.Flush();
            newGjf.Close();

            //产生第二个态的Gjf文件
            if (OS.OS.osClass == "windows")
            {
                newGjf = File.CreateText("tmp\\" + "State2_Freq" + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State2_Freq" + ".gjf");
            }

            for (int i = 0; i < gjf2Segment.firstSection.Count; i++)
            {
                newGjf.Write(gjf2Segment.firstSection[i] + "\n");
            }
            for (int i = 0; i < gjf2Segment.routeSection.Count; i++)
            {
                if (i == gjf2Segment.routeSection.Count - 1)
                {
                    newGjf.Write(gjf2Segment.routeSection[i] + " freq=noraman IOP(7/33=1)" + "\n");
                }
                else
                {
                    newGjf.Write(gjf2Segment.routeSection[i] + "\n");
                }
            }
            newGjf.Write("\n");
            for (int i = 0; i < gjf2Segment.titleSection.Count; i++)
            {
                newGjf.Write(gjf2Segment.titleSection[i] + "\n");
            }
            newGjf.Write("\n");
            newGjf.Write(gjf2Segment.chargeAndMultiplicity + "\n");
            //坐标
            if (gjf2Segment.coordinateType == "z-matrix")
            {
                for (int i = 0; i < gjf2Segment.molecularSpecification_ZMatrix.Count; i++)
                {
                    newGjf.Write(gjf2Segment.molecularSpecification_ZMatrix[i] + "\n");
                }
                newGjf.Write("\n");
                for (int i = 0; i < gjf2Segment.molecularPara_ZMatrix.Count; i++)
                {
                    newGjf.Write(data_MECP.functionData.para[i].PadRight(10) + "=     " + strX[i] + "\n");
                }
            }
            if (gjf2Segment.coordinateType == "cartesian")
            {
                for (int i = 0; i < gjf2Segment.molecularCartesian.Count; i++)
                {
                    newGjf.Write(gjf1Segment.molecularCartesian[i].Substring(0, 3).PadRight(10) + strX[3 * i].PadRight(15) + strX[3 * i + 1].PadRight(10) + strX[3 * i + 2].PadRight(10) + "\n");
                }
            }
            //附加部分
            newGjf.Write("\n");
            for (int i = 0; i < gjf2Segment.addition.Count; i++)
            {
                newGjf.Write(gjf2Segment.addition[i] + "\n");
            }
            newGjf.Flush();
            newGjf.Close();

            return;
        }

        /// <summary>
        /// 计算单点
        /// </summary>
        /// <param name="">输入数据</param>
        private void CalculateFreqSinglePoints_Gaussian(Data_Input data_Input)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //改变当前目录
            if (OS.OS.osClass == "windows")
            {
                Directory.SetCurrentDirectory(currentDirectory + "\\tmp");
            }
            else
            {
                Directory.SetCurrentDirectory(currentDirectory + "//tmp");
            }

            //运行高斯
            try
            {
                Process RunGaussian09 = new Process();
                //计算第一个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "State1_Freq" + ".gjf" + " " + "State1_Freq" + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第二个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "State2_Freq" + ".gjf" + " " + "State2_Freq" + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
            }
            catch
            {
                Console.WriteLine("MECP.RunMECP_1_CalculateSinglePoints.Gaussian Error." + "\n");
                Output.WriteOutput.Error.Append("MECP.RunMECP_1_CalculateSinglePoints.Gaussian Error." + "\n");
            }
            //回到原始目录
            Directory.SetCurrentDirectory(currentDirectory);
            return;
        }

        /// <summary>
        /// 获取振动计算数据
        /// </summary>
        /// <param name="data_Input"></param>
        /// <param name="data_MECP"></param>
        private void ObtainFreqCalculatingData_Gaussian_zMatrix(Data_Input data_Input, ref Data_MECP data_MECP)
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
                readGaussianOut = new ReadGaussianOut("tmp\\State1_Freq" + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State1_Freq" + ".out");
            }

            //检验N值
            N = readGaussianOut.N;
            if (data_Input.gaussianInputSegment.N != N)
            {
                Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
            }

            //获取能量
            switch (data_Input.mecpData.scfTyp1)
            {
                case "hftyp":
                    data_MECP.functionData.energy1 = readGaussianOut.GetHFEnergy(); ;
                    break;
                case "cistyp":
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
                    tmpHessian = readGaussianOut.GetForceConstant_Zmatrix();
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
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
                    tmpHessian = readGaussianOut.GetForceConstant_Cartesian();
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
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
                readGaussianOut = new ReadGaussianOut("tmp\\State2_Freq" + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State2_Freq" + ".out");
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
                    tmpHessian = readGaussianOut.GetForceConstant_Zmatrix();
                    for (int i = 0; i < 3 * N - 6; i++)
                    {
                        for (int j = 0; j < 3 * N - 6; j++)
                        {
                            tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                        }
                    }
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
                    tmpHessian = readGaussianOut.GetForceConstant_Cartesian();
                    for (int i = 0; i < 3 * N; i++)
                    {
                        for (int j = 0; j < 3 * N; j++)
                        {
                            tmpHessianDouble[i, j] = Convert.ToDouble(tmpHessian[i, j]);
                        }
                    }
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

            return;
        }


        private void ObtainFreqCalculatingData_Gaussian(Data_Input data_Input, ref Data_MECP.LiuFreq liuFreq)
        {
            ReadGaussianOut readGaussianOut;
            int N;
            double[,] tmpX;
            double[] tmpForce;
            double[,] tmpHessian;
            int i, j;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////                       读第一个态                              /////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (OS.OS.osClass == "windows")
            {
                readGaussianOut = new ReadGaussianOut("tmp\\State1_Freq" + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State1_Freq" + ".out");
            }

            //检验N值
            N = readGaussianOut.N;
            if (data_Input.gaussianInputSegment.N != N)
            {
                Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
            }
            int dim = 3 * N;

            //获取能量
            switch (data_Input.mecpData.scfTyp1)
            {
                case "hftyp":
                    liuFreq.energy1 = readGaussianOut.GetHFEnergy(); ;
                    break;
                case "cistyp":
                    break;
                default:
                    Output.WriteOutput.m_Result.Append("Get Energy Error.");
                    break;
            }

            //初始化参数
            tmpX = new double[N, 3];
            tmpForce = new double[dim];
            tmpHessian = new double[dim, dim];

            //初始化liuFreq中的参数
            liuFreq.atomicNumber = new int[N];
            liuFreq.atomicType = new int[N];            
            liuFreq.N = N;
            liuFreq.x = new BnulkMatrix(dim, 3);
            liuFreq.gradient1 = new BnulkVec(dim);
            liuFreq.hessian1 = new BnulkMatrix(dim, dim);
            liuFreq.gradient2 = new BnulkVec(dim);
            liuFreq.hessian2 = new BnulkMatrix(dim, dim);

            //读坐标梯度和力常数
            readGaussianOut.ReadInputOrientation(out liuFreq.atomicNumber, out liuFreq.atomicType, out tmpX);
            readGaussianOut.ReadInputOrientationForce(out tmpForce);
            readGaussianOut.ReadL703Hessian(out tmpHessian);

            //填入数据
            for(i=0;i<N;i++)
            {
                for(j=0;j<3;j++)
                {
                    liuFreq.x[i, j] = tmpX[i, j];
                }
            }
            for(i=0;i<dim;i++)
            {
                liuFreq.gradient1[i] = tmpForce[i];
            }
            for(i=0;i<dim;i++)
            {
                for(j=0;j<dim;j++)
                {
                    liuFreq.hessian1[i, j] = tmpHessian[i, j];
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////                       读第二个态                              /////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (OS.OS.osClass == "windows")
            {
                readGaussianOut = new ReadGaussianOut("tmp\\State2_Freq" + ".out");
            }
            else
            {
                readGaussianOut = new ReadGaussianOut("tmp//State2_Freq" + ".out");
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
                    liuFreq.energy2 = readGaussianOut.GetHFEnergy(); ;
                    break;
                case "cistyp":
                    break;
                default:
                    Output.WriteOutput.m_Result.Append("Get Energy Error.");
                    break;
            }

            //读坐标梯度和力常数
            readGaussianOut.ReadInputOrientation(out liuFreq.atomicNumber, out liuFreq.atomicType, out tmpX);
            readGaussianOut.ReadInputOrientationForce(out tmpForce);
            readGaussianOut.ReadL703Hessian(out tmpHessian);

            //填入数据
            for (i = 0; i < dim; i++)
            {
                liuFreq.gradient2[i] = tmpForce[i];
            }
            for (i = 0; i < dim; i++)
            {
                for (j = 0; j < dim; j++)
                {
                    liuFreq.hessian2[i, j] = tmpHessian[i, j];
                }
            }

            return;
        }


    }
}
