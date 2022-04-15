using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ChemKun.Data;
using ChemKun.Gaussian;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void CreateInputFiles(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            switch(data_Input.kunData.calProgram)
            {
                case "gaussian":
                    //是否计算Hessian阵
                    if (Math.IEEERemainder(data_MECP.I, data_Input.mecpData.hessianN) == 0)
                    {
                        data_MECP.isHessian = true;
                    }
                    else
                    {
                        data_MECP.isHessian = false;
                    }
                    //创建输入文件
                    if (data_MECP.isConvergence == false)
                    {
                        if (data_MECP.I == 0)
                        {
                            CreateFirstInputFiles_Gaussian(data_Input.gaussianInputSegment, data_MECP);
                        }
                        else
                        { 
                            CreateInputFiles_Gaussian(data_Input.gaussianInputSegment, data_MECP);                                      //创建中间文件
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        private void CreateFirstInputFiles_Gaussian(Data_Input.GaussianInputSegment gaussianInputSegment, Data_MECP data_MECP)
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

            //产生第一个态的Gjf文件
            if(OS.OS.osClass == "windows")
            {
                newGjf = File.CreateText("tmp\\" + "State1_" + data_MECP.I.ToString() + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State1_" + data_MECP.I.ToString() + ".gjf");
            }
            

            for(int i=0;i<gjf1Segment.firstSection.Count;i++)
            {
                newGjf.Write(gjf1Segment.firstSection[i] + "\n");
            }
            for(int i=0;i<gjf1Segment.routeSection.Count;i++)
            {
                StringBuilder additionalInformationOfRouteSection = new StringBuilder();

                if (i == gjf1Segment.routeSection.Count - 1)
                {
                    //处理是否计算Hessian
                    if (data_MECP.isHessian == true)
                        additionalInformationOfRouteSection.Append(" freq=noraman IOP(7/33=1)");
                    else
                        additionalInformationOfRouteSection.Append(" force IOP(7/33=1)");
                    //处理自洽场方法设置问题
                    if(data_MECP.scfMethod1!="")
                    {
                        additionalInformationOfRouteSection.Append(" " + data_MECP.scfMethod1);
                    }

                    newGjf.Write(gjf1Segment.routeSection[i] + additionalInformationOfRouteSection.ToString() + "\n");
                }
                else
                {
                    newGjf.Write(gjf1Segment.routeSection[i] + "\n");
                }
            }
            newGjf.Write("\n");
            for (int i=0;i<gjf1Segment.titleSection.Count;i++)
            {
                newGjf.Write(gjf1Segment.titleSection[i] + "\n");
            }
            newGjf.Write("\n");
            newGjf.Write(gjf1Segment.chargeAndMultiplicity + "\n");
            //坐标
            if(gjf1Segment.coordinateType=="z-matrix")
            {
                for(int i=0;i<gjf1Segment.molecularSpecification_ZMatrix.Count;i++)
                {
                    newGjf.Write(gjf1Segment.molecularSpecification_ZMatrix[i] + "\n");
                }
                newGjf.Write("\n");
                for (int i = 0; i < gjf1Segment.molecularPara_ZMatrix.Count; i++)
                {
                    newGjf.Write(gjf1Segment.molecularPara_ZMatrix[i] + "\n");
                }
            }
            if(gjf1Segment.coordinateType=="cartesian")
            {
                for (int i = 0; i < gjf1Segment.molecularCartesian.Count; i++)
                {
                    newGjf.Write(gjf1Segment.molecularCartesian[i] + "\n");
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
                newGjf = File.CreateText("tmp\\" + "State2_" + data_MECP.I.ToString() + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State2_" + data_MECP.I.ToString() + ".gjf");
            }
            
            for (int i = 0; i < gjf2Segment.firstSection.Count; i++)
            {
                newGjf.Write(gjf2Segment.firstSection[i] + "\n");
            }
            for (int i = 0; i < gjf2Segment.routeSection.Count; i++)
            {
                StringBuilder additionalInformationOfRouteSection = new StringBuilder();

                if (i == gjf2Segment.routeSection.Count - 1)
                {
                    //处理是否计算Hessian
                    if (data_MECP.isHessian == true)
                        additionalInformationOfRouteSection.Append(" freq=noraman IOP(7/33=1)");
                    else
                        additionalInformationOfRouteSection.Append(" force IOP(7/33=1)");
                    //处理自洽场方法设置问题
                    if (data_MECP.scfMethod2 != "")
                    {
                        additionalInformationOfRouteSection.Append(" " + data_MECP.scfMethod2);
                    }

                    newGjf.Write(gjf2Segment.routeSection[i] + additionalInformationOfRouteSection.ToString() + "\n");
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
                    newGjf.Write(gjf2Segment.molecularPara_ZMatrix[i] + "\n");
                }
            }
            if (gjf2Segment.coordinateType == "cartesian")
            {
                for (int i = 0; i < gjf2Segment.molecularCartesian.Count; i++)
                {
                    newGjf.Write(gjf2Segment.molecularCartesian[i] + "\n");
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


        private void CreateInputFiles_Gaussian(Data_Input.GaussianInputSegment gaussianInputSegment, Data_MECP data_MECP)
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

            //整数问题
            string[] strX = new string[data_MECP.functionData.x.Length];                                  //字符串形式的x     
            for(int i=0;i< data_MECP.functionData.x.Length; i++)
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
            //科学计数法问题
            for (int i = 0; i < data_MECP.functionData.x.Length; i++)
            {
                //如果Result[h]为整数，加上小数点
                if (Math.Abs(Convert.ToDouble(strX[i])) < 1E-4)
                {
                    strX[i] = "0.0";
                }
            }

            //产生第一个态的Gjf文件
            if (OS.OS.osClass == "windows")
            {
                newGjf = File.CreateText("tmp\\" + "State1_" + data_MECP.I.ToString() + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State1_" + data_MECP.I.ToString() + ".gjf");
            }            

            for (int i = 0; i < gjf1Segment.firstSection.Count; i++)
            {
                newGjf.Write(gjf1Segment.firstSection[i] + "\n");
            }
            for (int i = 0; i < gjf1Segment.routeSection.Count; i++)
            {
                StringBuilder additionalInformationOfRouteSection = new StringBuilder();
                if (i == gjf1Segment.routeSection.Count - 1)
                {
                    //处理是否计算Hessian
                    if (data_MECP.isHessian == true)
                        additionalInformationOfRouteSection.Append(" freq=noraman IOP(7/33=1)");
                    else
                        additionalInformationOfRouteSection.Append(" force IOP(7/33=1)");
                    //处理自洽场方法设置问题
                    if (data_MECP.scfMethod1 != "")
                    {
                        additionalInformationOfRouteSection.Append(" " + data_MECP.scfMethod1);
                    }

                    newGjf.Write(gjf1Segment.routeSection[i] + additionalInformationOfRouteSection.ToString() + "\n");
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
                newGjf = File.CreateText("tmp\\" + "State2_" + data_MECP.I.ToString() + ".gjf");
            }
            else
            {
                newGjf = File.CreateText("tmp//" + "State2_" + data_MECP.I.ToString() + ".gjf");
            }
            
            for (int i = 0; i < gjf2Segment.firstSection.Count; i++)
            {
                newGjf.Write(gjf2Segment.firstSection[i] + "\n");
            }
            for (int i = 0; i < gjf2Segment.routeSection.Count; i++)
            {
                StringBuilder additionalInformationOfRouteSection = new StringBuilder();
                if (i == gjf2Segment.routeSection.Count - 1)
                {
                    //处理是否计算Hessian
                    if (data_MECP.isHessian == true)
                        additionalInformationOfRouteSection.Append(" freq=noraman IOP(7/33=1)");
                    else
                        additionalInformationOfRouteSection.Append(" force IOP(7/33=1)");
                    //处理自洽场方法设置问题
                    if (data_MECP.scfMethod2 != "")
                    {
                        additionalInformationOfRouteSection.Append(" " + data_MECP.scfMethod2);
                    }

                    newGjf.Write(gjf2Segment.routeSection[i] + additionalInformationOfRouteSection.ToString() + "\n");
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

    }
}
