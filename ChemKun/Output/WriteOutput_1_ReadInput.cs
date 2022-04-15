using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        public static void WriteInputData(Data_Input data_Input)
        {
            //Input.ReadInput()内容
            m_Result.Clear();
            //输入文件内容标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-Input Info" + "\n");
            m_Result.Append("*********************************************" + "\n\n");
            //输入文件部分
            m_Result.Append("Associated program is " + data_Input.kunData.calProgram + "\n");
            m_Result.Append("Cmd format is " + data_Input.kunData.cmd + "\n");
            m_Result.Append("Task is " + data_Input.kunData.task + "\n");
            m_Result.Append("InputList: " + "\n");
            for (int i=0;i<data_Input.inputList.Count;i++)
            {
                m_Result.Append(data_Input.inputList[i] + "\n");
            }

            //Input.ReadInput_0_Kun()内容
            //输入文件内容标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-TaskAndKeyword Info" + "\n");
            m_Result.Append("*********************************************" + "\n\n");

            switch (data_Input.kunData.task.ToLower())
            {
                case "mecp":
                    MecpInfo(data_Input);
                    break;
                default:
                    break;
            }
            WriteOutput.Write();
        }

        private static void MecpInfo(Data_Input data_Input)
        {
            m_Result.Append("<MECP>" + "\n");
            m_Result.Append("  " + "calTyp=" + data_Input.mecpData.calTyp.ToString());
            m_Result.Append("  " + "coordinateType=" + data_Input.mecpData.coordinateType.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "method=" + data_Input.mecpData.method.ToString());
            m_Result.Append("  " + "scfTyp1=" + data_Input.mecpData.scfTyp1.ToString());
            m_Result.Append("  " + "scfTyp2=" + data_Input.mecpData.scfTyp2.ToString());
            m_Result.Append("  " + "cyc=" + data_Input.mecpData.cyc.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "stepSize=" + data_Input.mecpData.stepSize.ToString());
            m_Result.Append("  " + "guessHessian=" + data_Input.mecpData.guessHessian.ToString());
            m_Result.Append("  " + "hessianN=" + data_Input.mecpData.hessianN.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "energycon=" + data_Input.mecpData.criterianEnergy.ToString());
            m_Result.Append("  " + "maxcon=" + data_Input.mecpData.criterianMax.ToString());
            m_Result.Append("  " + "rmscon=" + data_Input.mecpData.criterianRMS.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "Lambda=" + data_Input.mecpData.lambda.ToString());
            m_Result.Append("  " + "isReadFirst=" + data_Input.mecpData.isReadFirst.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "judgement=" + data_Input.mecpData.judgement.ToString());
            m_Result.Append("  " + "mecpFreq=" + data_Input.mecpData.mecpFreq.ToString());
            m_Result.Append("\n");
            m_Result.Append("  " + "sqp_tao=" + data_Input.mecpData.sqp_tao.ToString());
            m_Result.Append("\n");
            m_Result.Append("</MECP>" + "\n");
            return;
        }
    }
}
