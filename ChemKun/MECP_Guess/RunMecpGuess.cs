using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemKun.Data;

namespace ChemKun.MECP_Guess
{
    partial class RunMecpGuess
    {

        Data_MecpGuess data_MecpGuess = new Data_MecpGuess();

        public RunMecpGuess(Data_Input data_Input)
        {
            Initialize(data_Input, ref data_MecpGuess);

            data_MecpGuess.I = 0;
            //创造一个tmp目录，用来写临时文件
            Directory.CreateDirectory("tmp");
            //如果有liuk文件夹，则从文件夹liuk中读取chk文件。
            ReadChkFromLiukFold();
            CreateInputFiles(data_Input, ref data_MecpGuess);
            CalculateSinglePoints(data_Input, data_MecpGuess.I);
            ObtainCalculatingData(data_Input, ref data_MecpGuess);
            Opt(data_Input, ref data_MecpGuess);
            UpDateData(ref data_MecpGuess);
            //UpDateData(ref data_MecpGuess);
            //TerminationCriteria(data_MecpGuess);

            for (data_MecpGuess.I=1; data_MecpGuess.isConvergence == false && data_MecpGuess.I <= data_Input.mecpGuessData.cyc; data_MecpGuess.I++)
            { 
                CreateInputFiles(data_Input, ref data_MecpGuess);
                CalculateSinglePoints(data_Input, data_MecpGuess.I);
                ObtainCalculatingData(data_Input, ref data_MecpGuess);
                data_MecpGuess.isConvergence = TerminationCriteria(data_MecpGuess);
                //输出部分：“输入文件”的信息
                Output.WriteOutput.WriteMecpGuess(data_Input, data_MecpGuess);
                //检查错误
                if (Output.WriteOutput.CheckError() == false)
                    return;

                Opt(data_Input, ref data_MecpGuess);
                UpDateData(ref data_MecpGuess);
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data_Input">输入的数据</param>
        private void Initialize(Data_Input data_Input, ref Data_MecpGuess data_MecpGuess)
        {
            //优化过程中用的坐标
            if (data_Input.mecpGuessData.coordinateType == "this")
            {
                switch (data_Input.kunData.calProgram)
                {
                    case "gaussian":
                        data_MecpGuess.functionData.coordinateType = data_Input.gaussianInputSegment.coordinateType;
                        break;
                    default:
                        break;
                }
            }
            
            data_MecpGuess.scfMethod1=data_Input.mecpGuessData.scfMethod1;
            data_MecpGuess.scfMethod2=data_Input.mecpGuessData.scfMethod2;
            data_MecpGuess.I = 0;
            data_MecpGuess.isConvergence = false;
            data_MecpGuess.criteria.deltaEnergy = data_Input.mecpGuessData.criterianEnergy;
            data_MecpGuess.functionData.scfTyp1 = data_Input.mecpGuessData.scfTyp1;
            data_MecpGuess.functionData.scfTyp2 = data_Input.mecpGuessData.scfTyp2;
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
                        if (OS.OS.osClass == "windows")
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
