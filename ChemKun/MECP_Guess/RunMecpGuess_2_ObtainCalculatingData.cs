using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.Gaussian;
using ChemKun.Estimate;

namespace ChemKun.MECP_Guess
{
    partial class RunMecpGuess
    {
        private void ObtainCalculatingData(Data_Input data_Input, ref Data_MecpGuess data_MecpGuess)
        {
            switch (data_Input.kunData.calProgram)
            {
                case "gaussian":
                    ObtainCalculatingData_Gaussian(data_Input, ref data_MecpGuess);
                    break;
                default:
                    break;
            }
            return;
        }

        private void ObtainCalculatingData_Gaussian(Data_Input data_Input, ref Data_MecpGuess data_MecpGuess)
        {
            ReadGaussianOut readGaussianOut;
            int N;
            string[,] tmpParams;

            if(data_MecpGuess.I==0)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第一个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\1_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//1_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }
                else
                {
                    data_MecpGuess.functionData.N = N;
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp1)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y1 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y1 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y1 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y1 = readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }


                switch (data_Input.gaussianInputSegment.coordinateType)                           //根据坐标类型，初始化参数
                {
                    case "z-matrix":
                        //初始化data_MecpGuess中的参数
                        data_MecpGuess.functionData.para = new string[3 * N - 6];
                        data_MecpGuess.functionData.x1 = new double[3 * N - 6];
                        data_MecpGuess.newX = new double[3 * N - 6];

                        //读坐标
                        tmpParams = readGaussianOut.GetForceParams_Zmatrix();
                        //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            data_MecpGuess.functionData.para[i] = tmpParams[i, 0];
                            data_MecpGuess.functionData.x1[i] = Convert.ToDouble(tmpParams[i, 1]);
                        }
                        data_MecpGuess.functionData.N = N;
                        break;
                    case "cartesian":
                        break;
                    default:
                        break;
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第二个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\2_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//2_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp1)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y2 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y2 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y2 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y2 = readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }


                switch (data_Input.gaussianInputSegment.coordinateType)                           //根据坐标类型，初始化参数
                {
                    case "z-matrix":
                        //初始化data_MecpGuess中的参数
                        data_MecpGuess.functionData.para = new string[3 * N - 6];
                        data_MecpGuess.functionData.x2 = new double[3 * N - 6];
                        data_MecpGuess.newX = new double[3 * N - 6];

                        //读坐标
                        tmpParams = readGaussianOut.GetForceParams_Zmatrix();
                        //根据计算结果，更新data_MECP.functionData中的坐标和梯度
                        for (int i = 0; i < 3 * N - 6; i++)
                        {
                            data_MecpGuess.functionData.para[i] = tmpParams[i, 0];
                            data_MecpGuess.functionData.x2[i] = Convert.ToDouble(tmpParams[i, 1]);
                        }
                        data_MecpGuess.functionData.N = N;
                        break;
                    case "cartesian":
                        break;
                    default:
                        break;
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第三个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\3_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//3_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp2)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y3 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y3 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y3 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y3 = readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }



                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第四个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\4_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//4_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp2)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y4 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y4 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y4 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y4 = readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }
            }
            else
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第五个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\5_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//5_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp2)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y5 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y5 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y5 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y5=readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }



                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////                       读第六个计算结果                        /////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (OS.OS.osClass == "windows")
                {
                    readGaussianOut = new ReadGaussianOut("tmp\\6_" + data_MecpGuess.I.ToString() + ".out");
                }
                else
                {
                    readGaussianOut = new ReadGaussianOut("tmp//6_" + data_MecpGuess.I.ToString() + ".out");
                }

                //检验N值
                N = readGaussianOut.N;
                if (data_Input.gaussianInputSegment.N != N)
                {
                    Console.WriteLine("ObtainCalculatingData_Gaussian: Read number of atom error");
                    Output.WriteOutput.Error.Append("ObtainCalculatingData_Gaussian: Read number of atom error");
                }

                //获取能量
                switch (data_Input.mecpGuessData.scfTyp2)
                {
                    case "hftyp":
                        data_MecpGuess.functionData.y6 = readGaussianOut.GetHFEnergy();
                        break;
                    case "cistyp":
                        data_MecpGuess.functionData.y6 = readGaussianOut.GetCISEnergy();
                        break;
                    case "tdtyp":
                        data_MecpGuess.functionData.y6 = readGaussianOut.GetTDEnergy();
                        break;
                    case "tdatyp":
                        data_MecpGuess.functionData.y6=readGaussianOut.GetTDAEnergy();
                        break;
                    default:
                        Output.WriteOutput.m_Result.Append("Get Energy Error.");
                        break;
                }
            }
            

            return;
        }
    }
}
