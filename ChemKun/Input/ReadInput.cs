using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ChemKun.Data;

namespace ChemKun.Input
{
    partial class ReadInput
    {
        public ReadInput(string inputFileName, ref Data_Input data_Input)
        {
            ReadDataFromInput(inputFileName, ref data_Input.inputList);                                            //输入：输入文件；输出：“输入列表”。                  
            JudgeCalProgram(data_Input.inputList, ref data_Input.kunData.calProgram);                              //输入：“输入列表”；输出：关联程序名称。
            //下面的函数在文件ReadInput_0_Kun中
            ReadKunKeyWordAndPara(data_Input.inputList, ref data_Input);                                           //输入：“输入列表”；输出：本程序的关键词和参数。

            DisposeInputList(data_Input.inputList, data_Input.kunData.calProgram, ref data_Input);                 //输入：“输入列表”；输出：关联程序的输入数据
        }

        /// <summary>
        /// 根据输入文件，初始化Data_Input中的参数：输入列表inputList
        /// </summary>
        /// <param name="inputFileName">输入文件名字</param>
        /// <param name="inputList">输入列表</param>
        private void ReadDataFromInput(string inputFileName, ref List<string> inputList)
        {
            //打开输入文件，即打开控制文件
            StreamReader inputFile = File.OpenText(inputFileName);
            string str = "";                                //临时用字符串，读一行文本
            inputList = new List<string>();
            while (inputFile.Peek() > -1)
            {
                str = inputFile.ReadLine();
                str = str.Trim();
                inputList.Add(str);
            }
            inputFile.Dispose();
            return;
        }

        /// <summary>
        /// 根据输入文件列表，判断计算选用的程序
        /// </summary>
        /// <param name="inputList">输入列表</param>
        /// <param name="calProgram">计算选用的程序</param>
        private void JudgeCalProgram(List<string> inputList, ref string calProgram)
        {
            //寻找Guassian程序标识
            calProgram = inputList[1].ToString().Trim();     //第一行
            calProgram = calProgram.Substring(0, 1);         //取第一行的第一个字符，如果是“%”，则判断为高斯程序。
            if (calProgram == "%")
            {
                calProgram = "gaussian";
            }
            else
            {
                calProgram = null;
            }
            return;
        }

        /// <summary>
        /// 把输入列表中的数据，结合关联程序，传递给：1、计算任务；2、任务列表；3、计算任务参数。
        /// </summary>
        /// <param name="inputList">输入列表</param>
        /// <param name="calProgram">关联程序</param>
        /// <param name="data_Input">输入数据，包括1、计算任务名称；2、计算任务的关键词和参数；3、计算任务输入文件的分段三部分</param>
        private void DisposeInputList(List<string> inputList, string calProgram, ref Data_Input data_Input)
        {
            switch (calProgram)
            {
                case "gaussian":
                    //函数在文件ReadInput_1_gaussian中
                    DisposeGaussianInputList(inputList, ref data_Input.gaussianInputSegment); 
                    break;
                default:
                    break;
            }
            return;
        }
    }
}
