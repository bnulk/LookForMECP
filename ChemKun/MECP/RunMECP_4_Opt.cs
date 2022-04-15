using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP.Opter;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2018-01-21

        描述：
            * 输入：Data_MECP中 -- 两个态势能函数的x，g1，g2，h1，h2，以及上一步的势能函数oldX，oldG1，oldG2，oldH1，oldH2。
            * 输入：Data_Input中 -- 坐标系coordinateType和原子个数N，决定了输入变量的维数。
            * 输出：Data_MECP中 -- 新的newX。
        结构：
            * Data_MECP ---- MECP计算相关的数据
        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */

        private void Opt(Data_Input data_Input, ref Data_MECP data_MECP)
        {
            switch (data_Input.mecpData.method)                           //根据坐标类型，初始化参数
            {
                case "ln":
                    switch(data_MECP.functionData.coordinateType)
                    {
                        case "z-matrix":
                            LagrangeNewton_Zmatrix lagrangeNewton_Zmatrix = new LagrangeNewton_Zmatrix(data_Input, ref data_MECP);
                            break;
                        case "cartesian":
                            LagrangeNewton_Cartesian lagrangeNewton_Cartesian = new LagrangeNewton_Cartesian(data_Input, ref data_MECP);
                            break;
                        default:
                            Output.WriteOutput.Error.Append("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            Console.WriteLine("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            break;
                    }
                    break;
                case "sqp":
                    switch (data_MECP.functionData.coordinateType)
                    {
                        case "z-matrix":
                            LagrangeSQP_Zmatrix lagrangeSQP_Zmatrix = new LagrangeSQP_Zmatrix(data_Input, ref data_MECP);
                            break;
                        case "cartesian":
                            LagrangeNewton_Cartesian lagrangeNewton_Cartesian = new LagrangeNewton_Cartesian(data_Input, ref data_MECP);
                            break;
                        default:
                            Output.WriteOutput.Error.Append("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            Console.WriteLine("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            break;
                    }
                    break;
                case "caln":
                    switch (data_MECP.functionData.coordinateType)
                    {
                        case "z-matrix":
                            CALN_Zmatrix cALN_Zmatrix = new CALN_Zmatrix(data_Input, ref data_MECP);
                            break;
                        case "cartesian":
                            LagrangeNewton_Cartesian lagrangeNewton_Cartesian = new LagrangeNewton_Cartesian(data_Input, ref data_MECP);
                            break;
                        default:
                            Output.WriteOutput.Error.Append("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            Console.WriteLine("can not find data_Input.gaussianInputSegment.coordinateType, ChemKun.MECP.RunMECP Error" + "/n");
                            break;
                    }
                    break;
                default:
                    break;
            }
            return;
        }
    }
}
