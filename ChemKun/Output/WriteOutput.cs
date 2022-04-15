using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        /*
       ----------------------------------------------------  类注释  开始----------------------------------------------------
       版本：  V1.0
       作者：  刘鲲
       日期：  2016-6-27

       描述：
           * 输出数据的类
       结构：
           * 
       ----------------------------------------------------  类注释  结束----------------------------------------------------

       */

        //全局变量        
        public static StringBuilder m_Result = new StringBuilder("");        //要输出的内容        
        public static StringBuilder Error;             //显示错误用
        public static string outputName = "Result.Kun";                      //默认输出文件名

        /// <summary>
        /// 创建新文件，并写入m_Result中的内容
        /// </summary>
        /// <param name="outputName">新文件的名字</param>
        public static void Write(string newOutputName)
        {
            try
            {
                outputName = newOutputName;
                //获取输出文件名字
                StreamWriter createLogFile = File.CreateText(outputName);
                createLogFile.Write(m_Result);
                createLogFile.Flush();
                createLogFile.Dispose();
            }
            catch
            {
                Console.WriteLine("ChemGo.Output.WriteOutput::Write(string outputName) died");
            }
        }

        /// <summary>
        /// 在当前打开的写入文件中，继续写入内容。
        /// </summary>
        public static void Write()
        {
            try
            {
                FileStream fs = new FileStream(outputName, FileMode.Open, FileAccess.Write);
                StreamWriter writeLogFile = new StreamWriter(fs);
                //writeLogFile.BaseStream.Seek(0, SeekOrigin.End);                        // 字符追加的位置
                writeLogFile.BaseStream.Position = fs.Length;                             // 字符追加的位置，在文件的最后。

                writeLogFile.Write(m_Result);
                writeLogFile.Flush();
                writeLogFile.Dispose();
            }
            catch
            {
                Console.WriteLine("ChemGo.Output.WriteOutput::Write() died");
            }
        }

        /// <summary>
        /// 检查计算中是否有出错提示。如果有，改变输出字符串对象为出错提示，并输出文本。
        /// </summary>
        public static bool CheckError()
        {
            if (Error != null)
            {
                WriteError(Error.ToString());
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// 在当前打开的写入文件中，写入出错提示
        /// </summary>
        /// <param name="Error">错误内容</param>
        private static void WriteError(string Error)
        {
            try
            {
                FileStream fs = new FileStream(outputName, FileMode.Open, FileAccess.Write);
                StreamWriter writeLogFile = new StreamWriter(fs);
                //writeLogFile.BaseStream.Seek(0, SeekOrigin.End);                        // 字符追加的位置
                writeLogFile.BaseStream.Position = fs.Length;                             // 字符追加的位置，在文件的最后。

                writeLogFile.Write("\n");
                writeLogFile.Write("-Error-Error-Error-Error-Error-Error-Error-Error-Error-Error-" + "\n");
                writeLogFile.Write("Error:" + "\n");
                writeLogFile.Write(Error);
                writeLogFile.Write("-Error-Error-Error-Error-Error-Error-Error-Error-Error-Error-" + "\n");
                writeLogFile.Flush();
                writeLogFile.Dispose();
            }
            catch
            {
                Console.WriteLine("ChemGo.Output.WriteOutput::WriteError() died");
            }
        }

        /// <summary>
        /// 在当前打开的写入文件中，写入一行。
        /// </summary>
        public static void WriteStr(string str)
        {
            m_Result.Clear();
            m_Result.Append(str);
            Write();
            return;
        }

        /// <summary>
        /// 在当前打开的写入文件中，写入字符序列。
        /// </summary>
        public static void WriteStr(StringBuilder strB)
        {
            m_Result.Clear();
            m_Result = strB;
            Write();
            return;
        }

    }
}
