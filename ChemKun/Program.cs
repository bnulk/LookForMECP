using System;
using ChemKun.Data;
using ChemKun.Output;

namespace ChemKun
{
    partial class Program
    {
        ///全局变量data_Cmd和data_Input。命令行输入和文本输入的数据
        static Data_Cmd data_Cmd = new Data_Cmd();
        static Data_Input data_Input = new Data_Input();

        static void Main(string[] args)
        {
            ///获取操作系统信息
            OS.OS.osClass = OS.OS.ObtianOsClass();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                               输入：命令行。输出：命令行数据                                 ///
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            //处理输入命令行，获取输入文件名、输出文件名和一个参数
            Input.HandleCmdLine handleCmdLine = new Input.HandleCmdLine(args, ref data_Cmd.cmdData);
            //创造一个输出文件
            Output.WriteOutput.Write(data_Cmd.cmdData.outputName);
            //输出程序的Title信息
            Output.WriteOutput.WriteTitle();
            //输出命令行信息
            Output.WriteOutput.WriteCmdLine(data_Cmd.cmdData);


            ////////////////////////////////////////////////////////////////////////////////////////////////////
            ///       输入：输入文件。输出：关联程序，输入列表，计算任务，计算任务的关键词和参数             ///
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            //从输入文件中读入数据
            Input.ReadInput readInput = new Input.ReadInput(data_Cmd.cmdData.inputName, ref data_Input);
            //输出部分：“输入文件”的信息
            Output.WriteOutput.WriteInputData(data_Input);
            //检查错误
            if (Output.WriteOutput.CheckError() == false)
                return;


            ////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                                  根据计算任务分配计算流程                                    ///
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            BuildTaskList(data_Input.kunData.task, data_Input);              //在BuildTaskList.cs文件中
            return;
        }
    }
}
