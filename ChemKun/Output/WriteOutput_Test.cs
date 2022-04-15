using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        public static void WriteTest(string str)
        {
            m_Result.Clear();
            //输入文件内容标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-MECP Info" + "\n");
            m_Result.Append("*********************************************" + "\n\n");
            //输入文件部分
            m_Result.Append("Test is: " + "\n");

            m_Result.Append(str.ToString());

            WriteOutput.Write();
            return;
        }
    }
}
