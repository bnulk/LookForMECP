using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.Input
{
    partial class ReadInput
    {
        /// <summary>
        /// 从高斯程序的routeSection中，读取本程序附加的关键词和参数，即{}中的部分，填入Data_Input对象相应的部分中
        /// </summary>
        /// <param name="routeSection"></param>
        /// <param name="data_Input"></param>
        private void ReadKunKeyWordAndPara(List<string> inputList, ref Data_Input data_Input)
        {
            //获取{}中的内容
            int indexStart;
            int indexEnd;
            int indexMark;
            string str = "";
            for (int i = 0; i < inputList.Count; i++)
            {
                str += inputList[i].ToString();
            }
            indexStart = str.IndexOf('{');
            indexEnd = str.IndexOf('}');
            if (str.LastIndexOf('{') != indexStart)
            {
                Console.WriteLine("More than one { are found" + "/n");
                Output.WriteOutput.Error.Append("More than one { are found" + "/n");
            }
            if (indexStart == -1 || indexEnd == -1)
            {
                Console.WriteLine("Can not find the {} in the input file." + "/n");
                Output.WriteOutput.Error.Append("Can not find the {} in the input file." + "/n");
            }
            str = str.Remove(indexEnd, str.Length - indexEnd);
            str = str.Remove(0, indexStart + 1);

            //根据{}中的任务初始化data_Input对象中相应的数据
            //计算任务
            indexMark = str.IndexOf("task=");
            if (indexMark == -1)                                    //默认值
            {
                data_Input.kunData.task = "mecp";
            }
            else
            {
                data_Input.kunData.task = str.Remove(0, indexMark + 5);
                indexMark = data_Input.kunData.task.IndexOf(' ');
                if (indexMark != -1)
                {
                    data_Input.kunData.task = data_Input.kunData.task.Remove(indexMark, data_Input.kunData.task.Length - indexMark);
                }
            }
            //计算关联程序的命令
            indexMark = str.IndexOf("cmd=");
            if (indexMark == -1)                                    //默认值
            {
                data_Input.kunData.cmd = "g09";
            }
            else
            {
                data_Input.kunData.cmd = str.Remove(0, indexMark + 4);
                indexMark = data_Input.kunData.cmd.IndexOf(' ');
                if (indexMark != -1)
                {
                    data_Input.kunData.cmd = data_Input.kunData.cmd.Remove(indexMark, data_Input.kunData.cmd.Length - indexMark);
                }
            }
            //根据任务名称，初始化任务的关键词参数
            Initialize(data_Input.kunData.task, ref data_Input);
            //把{}中的内容填入data_Input对象中
            WriteKeyWordAndPara(str, ref data_Input);
            return;
        }

        private void Initialize(string task, ref Data_Input data_Input)
        {
            switch (task.ToLower())
            {
                case "mecp":
                    data_Input.mecpData.calTyp = "opt";
                    data_Input.mecpData.coordinateType = "this";
                    data_Input.mecpData.convergenceTyp = "normal";
                    data_Input.mecpData.criterianEnergy = Math.Pow(10, 5 * (-1));
                    data_Input.mecpData.criterianMax = 0.001;
                    data_Input.mecpData.criterianRMS = 0.0005;
                    data_Input.mecpData.cyc = 100;
                    data_Input.mecpData.file1 = "State1.gjf";
                    data_Input.mecpData.file2 = "State2.gjf";
                    data_Input.mecpData.file3 = "State3.gjf";
                    data_Input.mecpData.guessHessian = "bfgs";
                    data_Input.mecpData.hessianN = 1;
                    data_Input.mecpData.isReadFirst = false;
                    data_Input.mecpData.judgement = "global";
                    data_Input.mecpData.lambda = 1;
                    data_Input.mecpData.mecpFreq = "liu";
                    data_Input.mecpData.method = "ln";
                    data_Input.mecpData.scfMethod1 = "";
                    data_Input.mecpData.scfMethod2 = "";
                    data_Input.mecpData.scfTyp1 = "hftyp";
                    data_Input.mecpData.scfTyp2 = "hftyp";
                    data_Input.mecpData.showGradRatioCriterionN = 4;
                    data_Input.mecpData.showGradRatioCriterion = Math.Pow(10, data_Input.mecpData.showGradRatioCriterionN * (-1));     //实际还没有用到；显示梯度比，力阚值0.0001
                    data_Input.mecpData.stepSize = 1;
                    data_Input.mecpData.sqp_tao = 0.01;
                    data_Input.mecpData.check = true;
                    break;
                case "mecpguess":
                    data_Input.mecpGuessData.method = "lineapproximate";
                    data_Input.mecpGuessData.coordinateType = "this";
                    data_Input.mecpGuessData.scfMethod1 = "";
                    data_Input.mecpGuessData.scfMethod2 = "";
                    data_Input.mecpGuessData.scfTyp1 = "hftyp";
                    data_Input.mecpGuessData.scfTyp2 = "hftyp";
                    data_Input.mecpGuessData.criterianEnergy = Math.Pow(10, 5 * (-1));
                    data_Input.mecpGuessData.cyc = 100;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 把关键词和参数的输入信息，填入Data_Input对象相应的部分中。
        /// </summary>
        /// <param name="str">关键词和参数的输入信息</param>
        /// <param name="data_Input">关键词和参数的数据</param>
        private void WriteKeyWordAndPara(string str, ref Data_Input data_Input)
        {
            string[] strKeyWordAndPara;                             //分离关键词和参数
            strKeyWordAndPara = str.Split(' ');
            //从{}中确定任务
            string[] inputKeyWord = new string[2];           //指令数组，包括所有行，被“=”分成两部分或者一部分。不一定是有效指令
            for (int i = 0; i < strKeyWordAndPara.Length; i++)
            {
                strKeyWordAndPara[i] = strKeyWordAndPara[i].ToLower();
                try
                {
                    inputKeyWord = strKeyWordAndPara[i].Split('=');
                    for(int j=0; j< inputKeyWord.Length;j++)
                    {
                        inputKeyWord[j] = inputKeyWord[j].Replace('#', '=');
                    }
                }
                catch
                {
                    Output.WriteOutput.Error.Append("Input Error" + "\n" + inputKeyWord.ToString() + "\n");
                }
                if (inputKeyWord.Length >= 2)
                {
                    switch(data_Input.kunData.task.ToLower())
                    {
                        case "mecp":
                            WriteMECP(inputKeyWord, ref data_Input.mecpData);
                            break;
                        case "mecpguess":
                            WriteMecpGuess(inputKeyWord, ref data_Input.mecpGuessData);
                            break;
                        default:
                            WriteMECP(inputKeyWord, ref data_Input.mecpData);
                            break;
                    }
                    
                }
            }
            return;
        }

        /// <summary>
        /// 根据指令初始化
        /// </summary>
        /// <param name="InputKeyWord">指令数组</param>
        private void WriteMECP(string[] InputKeyWord, ref Data_Input.MecpData mecpData)
        {
            //根据输入文件的关键词初始化指令
            switch (InputKeyWord[0].ToLower())
            {
                case "method":
                    mecpData.method = InputKeyWord[1].ToLower();
                    break;
                case "caltyp":
                    mecpData.calTyp = InputKeyWord[1].ToLower();
                    break;
                case "file1":
                    mecpData.file1 = InputKeyWord[1];
                    break;
                case "file2":
                    mecpData.file2 = InputKeyWord[1];
                    break;
                case "file3":
                    mecpData.file3 = InputKeyWord[1];
                    break;
                case "scfmethod1":
                    mecpData.scfMethod1= InputKeyWord[1].ToLower();
                    break;
                case "scfmethod2":
                    mecpData.scfMethod2 = InputKeyWord[1].ToLower();
                    break;
                case "scftyp1":
                    mecpData.scfTyp1 = InputKeyWord[1].ToLower();
                    break;
                case "scftyp2":
                    mecpData.scfTyp2 = InputKeyWord[1].ToLower();
                    break;
                case "convergencetyp":
                    mecpData.convergenceTyp = InputKeyWord[1].ToLower();
                    if (mecpData.convergenceTyp == "loose")
                    {
                        mecpData.criterianEnergy = Math.Pow(10, 4 * (-1));
                        mecpData.criterianMax = 0.01;
                        mecpData.criterianRMS = 0.005;
                    }
                    break;
                case "energycon":
                    mecpData.criterianEnergy = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "maxcon":
                    mecpData.criterianMax = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "rmscon":
                    mecpData.criterianRMS = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "cyc":
                    mecpData.cyc = Convert.ToInt32(InputKeyWord[1]);
                    break;
                case "stepsize":
                    mecpData.stepSize = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "hessiann":
                    mecpData.hessianN = Convert.ToInt32(InputKeyWord[1]);
                    break;
                case "isreadfirst":
                    mecpData.isReadFirst = Convert.ToBoolean(InputKeyWord[1]);
                    break;
                case "guesshessian":
                    mecpData.guessHessian = InputKeyWord[1].ToLower();
                    break;
                case "lambda":
                    mecpData.lambda = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "judgement":
                    mecpData.judgement = InputKeyWord[1].ToLower();
                    break;
                case "mecpfreq":
                    mecpData.mecpFreq = InputKeyWord[1].ToLower();
                    break;
                case "showgradratiocriterionn":
                    mecpData.showGradRatioCriterionN = Convert.ToDouble(InputKeyWord[1]);
                    mecpData.showGradRatioCriterion = Math.Pow(10, mecpData.showGradRatioCriterionN * (-1));
                    break;
                case "sqp_tao":
                    mecpData.sqp_tao = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "check":
                    mecpData.check = Convert.ToBoolean(InputKeyWord[1]);
                    break;
                default:
                    break;
            }
            return;
        }

        private void WriteMecpGuess(string[] InputKeyWord, ref Data_Input.MecpGuessData mecpGuessData)
        {
            //根据输入文件的关键词初始化指令
            switch (InputKeyWord[0].ToLower())
            {
                case "method":
                    mecpGuessData.method = InputKeyWord[1].ToLower();
                    break;
                case "scfmethod1":
                    mecpGuessData.scfMethod1 = InputKeyWord[1].ToLower();
                    break;
                case "scfmethod2":
                    mecpGuessData.scfMethod2 = InputKeyWord[1].ToLower();
                    break;
                case "scftyp1":
                    mecpGuessData.scfTyp1 = InputKeyWord[1].ToLower();
                    break;
                case "scftyp2":
                    mecpGuessData.scfTyp2 = InputKeyWord[1].ToLower();
                    break;
                case "energycon":
                    mecpGuessData.criterianEnergy = Convert.ToDouble(InputKeyWord[1]);
                    break;
                case "cyc":
                    mecpGuessData.cyc = Convert.ToInt32(InputKeyWord[1]);
                    break;
                default:
                    break;
            }
            return;
        }



    }
}
