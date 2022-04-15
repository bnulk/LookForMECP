using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        /// <summary>
        /// 向输出文件中输出Title部分
        /// </summary>
        public static void WriteTitle()
        {
            m_Result.Clear();
            //程序来源
            m_Result.Append("PROGRAM LookForMECP, Version 2.1_20211209" + "\n" + "Liu Kun  2021-12-09" + "\n" + "\n");
            m_Result.Append(DateTime.Now.ToString() + "\n");
            m_Result.Append("*********************************************" + "\n");
            m_Result.Append("Author Information: Liu Kun, College of Chemistry, Tianjin Normal University, Tianjin 300387, China" + "\n");
            m_Result.Append("Email: bnulk@foxmail.com" + "\n");
            m_Result.Append("You can obtain the newest version of the program by contacting the author." + "\n");
            m_Result.Append("*********************************************" + "\n");
            WriteOutput.Write();
        }

        /// <summary>
        /// 文本输出“命令行参数”
        /// </summary>
        /// <param name="cmdData">命令行参数</param>
        public static void WriteCmdLine(Data_Cmd.CmdData cmdData)
        {
            m_Result.Clear();
            //输入的命令行标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-Cmd Info" + "\n");
            m_Result.Append("*********************************************" + "\n\n");
            //输入的命令行
            m_Result.Append("currentOS: " + Environment.OSVersion.ToString() + "\n");
            m_Result.Append("currentDirectory: " + cmdData.directoryName.ToString() + "\n");
            m_Result.Append("inputName: " + cmdData.inputName.ToString() + "\n");
            m_Result.Append("outputName: " + cmdData.outputName.ToString() + "\n");
            if (cmdData.param != null)
                m_Result.Append("param: " + cmdData.param.ToString() + "\n");
            WriteOutput.Write();
        }
    }
}
