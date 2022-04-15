using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ChemKun.Data;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        public RunMECP(Data_Input data_Input)
        {
            Data_MECP data_MECP = new Data_MECP();

            //优化过程中用的坐标
            if(data_Input.mecpData.coordinateType=="this")
            {
                switch(data_Input.kunData.calProgram)
                {
                    case "gaussian":
                        data_MECP.functionData.coordinateType = data_Input.gaussianInputSegment.coordinateType;
                        break;
                    default:
                        break;
                }
            }
            data_MECP.scfMethod1 = data_Input.mecpData.scfMethod1;
            data_MECP.scfMethod2 = data_Input.mecpData.scfMethod2;
            data_MECP.isReadFirst=data_Input.mecpData.isReadFirst;                        //是否读第一步的计算数据
            data_MECP.functionData.Lambda = data_Input.mecpData.lambda;                   //拉格朗日Lambd
            data_MECP.isHessian = true;                                                   //是否计算Hessian矩阵
            data_MECP.I = 0;                                                              //迭代的次数
            data_MECP.stepSize = data_Input.mecpData.stepSize;                            //步长
            data_MECP.isConvergence = false;                                              //迭代是否收敛
            data_MECP.criteria.criteriaEnergy = data_Input.mecpData.criterianEnergy;      //能量收敛标准
            data_MECP.criteria.criteriaMax = data_Input.mecpData.criterianMax;            //拉格朗日力收敛标准
            data_MECP.criteria.criteriaRMS = data_Input.mecpData.criterianRMS;            //均方根拉格朗日力收敛标准   
            data_MECP.mecpFreq = data_Input.mecpData.mecpFreq;                            //振动分析选项



            //创造一个tmp目录，用来写临时文件
            Directory.CreateDirectory("tmp");

            //如果有liuk文件夹，则从文件夹liuk中读取chk文件。
            ReadChkFromLiukFold();
            CreateInputFiles(data_Input, ref data_MECP);

            if (data_MECP.isReadFirst==false)
            {                
                CalculateSinglePoints(data_Input, data_MECP.I);
            }
            ObtainCalculatingData(data_Input, ref data_MECP);
            data_MECP.isConvergence = TerminationCriteria(data_MECP, data_Input.mecpData, ref data_MECP.criteria);
            
            //输出部分：“输入文件”的信息
            Output.WriteOutput.WriteMECP(data_Input, data_MECP);
            //检查错误
            if (Output.WriteOutput.CheckError() == false)
                return;

            for (int i=0; data_MECP.isConvergence == false && i<=data_Input.mecpData.cyc; i++)
            {
                Opt(data_Input, ref data_MECP);
                UpDateData(ref data_MECP); 
                ReadChkFromLiukFold();                                                    //如果有liuk文件夹，则从文件夹liuk中读取chk文件。
                CreateInputFiles(data_Input, ref data_MECP);
                CalculateSinglePoints(data_Input, data_MECP.I);

                ObtainCalculatingData(data_Input, ref data_MECP);

                data_MECP.isConvergence = TerminationCriteria(data_MECP, data_Input.mecpData, ref data_MECP.criteria);

                //输出部分：“输入文件”的信息
                Output.WriteOutput.WriteMECP(data_Input, data_MECP);
                //检查错误
                if (Output.WriteOutput.CheckError() == false)
                    return;
            }

            /*
            //如果收敛，则检查充分性
            if(data_MECP.isConvergence==true && data_Input.mecpData.check==true)
            {
                Freq(data_Input, data_MECP, ref data_MECP.freq);
            }
            */

            OutputResult(data_MECP);
            return;
        }

        /// <summary>
        /// 复制临时文件夹liuk中的chk文件。
        /// </summary>
        private void ReadChkFromLiukFold()
        {
            if (Directory.Exists("liuk"))
            {
                try
                {
                    string[] chkList = Directory.GetFiles("liuk", "*.chk");
                    foreach (string f in chkList)
                    {
                        //remove path from the file name
                        string fName = f.Substring(5);
                        if(OS.OS.osClass == "windows")
                        {
                            File.Copy("liuk\\" + fName, "tmp\\" + fName, true);
                        }
                        else
                        {
                            File.Copy("liuk//" + fName, "tmp//" + fName, true);
                        }

                    }
                }
                catch
                {
                    Output.WriteOutput.m_Result.Append("no copy chk from liuk." + "\n");
                    Console.WriteLine("no copy chk from liuk." + "\n");
                }
            }
            return;
        }
    }
}
