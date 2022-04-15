using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void OutputResult(Data_MECP data_MECP)
        {
            //输出部分：计算结果
            Output.WriteOutput.WriteMECPResult(data_MECP);
            /*
            //输出部分：振动分析的信息
            if (data_MECP.mecpFreq=="simple")
            {
                Output.WriteOutput.WriteMECPFreq(data_MECP);
            }
            else
            {
                Output.WriteOutput.WriteLiuFreq(data_MECP.isConvergence, data_MECP.liuFreq.isRealMECP);
            }
            */
            //检查错误
            if (Output.WriteOutput.CheckError() == false)
                return;
            return;
        }
    }
}
