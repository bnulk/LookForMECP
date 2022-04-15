using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void UpDateData(ref Data_MECP data_MECP)
        {
            switch (data_MECP.functionData.coordinateType)                           //根据坐标类型，初始化参数
            {
                case "z-matrix":
                    for (int i = 0; i < (data_MECP.functionData.N - 1); i++)     //原子参数（波尔）转为埃
                    {
                        data_MECP.functionData.x[i] = data_MECP.newX[i] * 0.529177249;                //波尔转埃
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);     //保留小数点后6位
                    }

                    for (int i = data_MECP.functionData.N - 1; i < (3 * data_MECP.functionData.N - 6); i++) //原子参数（弧度）转为度
                    {
                        data_MECP.functionData.x[i] = data_MECP.newX[i] * 180 / System.Math.PI;              //=180/3.1415927
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);            //保留小数点后6位
                    }
                    //新参数角度部分大于180或者小于0
                    for (int i = data_MECP.functionData.N - 1; i < (2 * data_MECP.functionData.N - 3); i++) //原子参数（弧度）转为度
                    {
                        if (data_MECP.functionData.x[i] > 180.0 || data_MECP.functionData.x[i] < 0.0)
                        {
                            Output.WriteOutput.m_Result.Append("Error. The new angle is greater than 180 degrees or less than 0 degrees." + "\n");
                            Console.WriteLine("Error. The new angle is greater than 180 degrees or less than 0 degrees." + "\n");
                        }

                    }
                    break;
                case "cartesian":
                    for (int i = 0; i < (3 * data_MECP.functionData.N); i++)     //原子参数（波尔）转为埃
                    {
                        data_MECP.functionData.x[i] = data_MECP.newX[i] * 0.529177249;                //波尔转埃
                        data_MECP.functionData.x[i] = Math.Round(data_MECP.functionData.x[i], 6);     //保留小数点后6位
                    }
                    break;
                default:
                    break;
            }
            return;
        }
    }
}
