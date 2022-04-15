using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP;
using ChemKun.MECP_Guess;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        /// <summary>
        /// MECP初始信息
        /// </summary>
        /// <param name="data_Input">输入数据</param>
        /// <param name="data_MECP">MECP数据</param>
        public static void WriteMecpGuess(Data_Input data_Input, Data_MecpGuess data_MecpGuess)
        {
            if (data_MecpGuess.I == 0)
            {
                m_Result.Clear();
                //输入文件内容标志
                m_Result.Append("\n");
                m_Result.Append("bnulk@foxmail.com-MECP Info" + "\n");
                m_Result.Append("*********************************************" + "\n\n");
                //输入文件部分
                m_Result.Append(" MecpGuess method is " + data_Input.mecpGuessData.method.ToString() + "\n");
            }
            else
            {
                m_Result.Clear();
            }

            WriteMecpGuessData(data_MecpGuess);

            WriteOutput.Write();
            return;
        }

        /// <summary>
        /// 计算过程中的输出
        /// </summary>
        /// <param name="data_MECP">MECP数据</param>
        public static void WriteMecpGuessData(Data_MecpGuess data_MecpGuess)
        {
            m_Result.Append("##########     I is:" + data_MecpGuess.I.ToString() + "     ##########" + "\n");
            m_Result.Append("The Energy of the First State is:" + data_MecpGuess.functionData.y5.ToString() + "\n");
            m_Result.Append("The Energy of the Second State is:" + data_MecpGuess.functionData.y6.ToString() + "\n");
            m_Result.Append("The Energy Difference between the Two States is:" + (data_MecpGuess.functionData.y5 - data_MecpGuess.functionData.y6).ToString() + "\n");
            //集中显示重要结果
            /*
            m_Result.Append("----------" + "\n");

            m_Result.Append("I".PadLeft(1).PadRight(10) + "detEnergy".PadLeft(10).PadRight(20) + "Lambda".PadLeft(6).PadRight(20) + "  AverEnergy".PadRight(30) + "MaxForce".PadRight(20) + "RMSForce".PadRight(20) + "\n");
            for (int i = 0; i < data_MECP.record.Count; i++)
            {
                m_Result.Append(data_MECP.record[i][0].PadRight(10));
                m_Result.Append(data_MECP.record[i][1].PadRight(20));
                m_Result.Append(data_MECP.record[i][2].PadRight(20));
                m_Result.Append(data_MECP.record[i][3].PadRight(30));
                m_Result.Append(data_MECP.record[i][4].PadRight(20));
                m_Result.Append(data_MECP.record[i][5].PadRight(20));
                m_Result.Append("\n");
            }
            */
           return;
        }

        public static void WriteMecpGuessResult(Data_MecpGuess data_MecpGuess)
        {
            /*
            m_Result.Clear();
            //输出结果内容标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-MECP Result" + "\n");
            m_Result.Append("*********************************************" + "\n\n");
            //输出结果内容
            m_Result.Append("Energy = " + ((data_MECP.functionData.energy1 + data_MECP.functionData.energy2) / 2).ToString() + "\n");
            m_Result.Append("Lambda = " + data_MECP.functionData.Lambda.ToString() + "\n");
            m_Result.Append("-Lambda/(1-Lambda) = " + (data_MECP.functionData.Lambda * (-1) / (1 - data_MECP.functionData.Lambda)).ToString() + "\n");
            m_Result.Append("Gradient ratio between two states: " + "\n");

            for (int i = 0; i < data_MECP.functionData.gradient1.Length; i++)
            {
                if (Math.Abs(data_MECP.functionData.gradient1[i]) > 0.01 && Math.Abs(data_MECP.functionData.gradient2[i]) > 0.01)
                {
                    m_Result.Append(data_MECP.functionData.para[i].PadRight(10) + "=     " + Math.Round(data_MECP.functionData.gradient1[i] / data_MECP.functionData.gradient2[i], 2).ToString().PadRight(10) + "\n");
                }
                else
                {
                    m_Result.Append(data_MECP.functionData.para[i].PadRight(10) + "=     " + "smallGradient".PadRight(10) + "\n");
                }
            }
            WriteOutput.Write();
            */
            return;
        }
    }
}
