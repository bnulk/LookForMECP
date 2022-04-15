using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.MECP_Guess
{
    partial class RunMecpGuess
    {
        private bool TerminationCriteria(Data_MecpGuess data_MecpGuess)
        {
            bool isConvergence = false;
            bool energyIsConvergence = false;                                                           //能量标准是否收敛
            //初始化计划填写的数据
            double deltaEnergy = data_MecpGuess.functionData.y5 - data_MecpGuess.functionData.y6;               //能量


            if (Math.Abs(deltaEnergy) < data_MecpGuess.criteria.deltaEnergy)                //根据能量，判断是否收敛
            {
                energyIsConvergence = true;
            }

            if (energyIsConvergence == true)
            {
                isConvergence = true;
            }

            /*
            //为了集中显示能量差和Lambda，给出记录。
            List<string> tmpList = new List<string>();
            tmpList.Add(data_MECP.I.ToString());                                                                                                   //步数标号
            tmpList.Add(Math.Round((data_MECP.functionData.energy1 - data_MECP.functionData.energy2), 8).ToString());                              //能量差
            tmpList.Add(Math.Round(data_MECP.functionData.Lambda, 8).ToString());                                                                  //Lambda
            tmpList.Add(Math.Round(((data_MECP.functionData.energy1 + data_MECP.functionData.energy2) / 2), 8).ToString());                        //平均能量
            tmpList.Add(Math.Round(data_MECP.criteria.maxLagrangeForce, 6).ToString());                                                            //最大Lagrange力
            tmpList.Add(Math.Round(data_MECP.criteria.RMSLagrangeForce, 6).ToString());                                                            //最大均方根Lagrange力
            data_MECP.record.Add(tmpList);
            */

            if((data_MecpGuess.functionData.y3-data_MecpGuess.functionData.y1)*(data_MecpGuess.functionData.y6-data_MecpGuess.functionData.y5)>0)
            {
                data_MecpGuess.functionData.y1 = data_MecpGuess.functionData.y5;
                data_MecpGuess.functionData.y3 = data_MecpGuess.functionData.y6;
                for(int i=0;i<data_MecpGuess.functionData.x1.Length;i++)
                {
                    data_MecpGuess.functionData.x1[i]=data_MecpGuess.functionData.x3[i];
                }
            }
            else
            {
                data_MecpGuess.functionData.y2 = data_MecpGuess.functionData.y5;
                data_MecpGuess.functionData.y4 = data_MecpGuess.functionData.y6;
                for (int i = 0; i < data_MecpGuess.functionData.x1.Length; i++)
                {
                    data_MecpGuess.functionData.x2[i] = data_MecpGuess.functionData.x3[i];
                }
            }


            return isConvergence;
        }
        
    }
}
