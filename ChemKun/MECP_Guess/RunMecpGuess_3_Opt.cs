using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP_Guess.GuessOpter;

namespace ChemKun.MECP_Guess
{
    partial class RunMecpGuess
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2021-11-20

        描述：
            * 输入：Data_MecpGuess中 -- 两个态势能函数的x1,x2,y1,y2,y3,y4
            * 输入：Data_Input中 -- 坐标系coordinateType和原子个数N，决定了输入变量的维数。
            * 输出：Data_MecpGuess中 -- 新的newX。
        结构：
            * Data_MECP ---- MECP计算相关的数据

        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */

        private void Opt(Data_Input data_Input, ref Data_MecpGuess data_MecpGuess)
        {
            switch (data_Input.mecpGuessData.method)                           //根据坐标类型，初始化参数
            {
                case "lineapproximate":
                    switch (data_MecpGuess.functionData.coordinateType)
                    {
                        case "z-matrix":
                            LineApproximate_Zmatrix lineApproximate_Zmatrix  = new LineApproximate_Zmatrix(data_Input, ref data_MecpGuess);
                            break;
                        case "cartesian":
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
