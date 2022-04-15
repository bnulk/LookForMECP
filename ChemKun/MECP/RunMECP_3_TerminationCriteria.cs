using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private bool TerminationCriteria(Data_MECP data_MECP, Data_Input.MecpData mecpData, ref Data_MECP.Criteria criteria)
        {
            bool isConvergence = false;                                                                 //是否收敛
            bool forceIsConvergence = false;                                                            //力标准是否收敛
            bool energyIsConvergence = false;                                                           //能量标准是否收敛
            //初始化计划填写的数据
            criteria.deltaEnergy = data_MECP.functionData.energy1 - data_MECP.functionData.energy2;               //能量
            criteria.lagrangeForce = new double[data_MECP.functionData.gradient1.Length];                         //拉格朗日力


            if(Math.Abs(criteria.deltaEnergy) < mecpData.criterianEnergy)                //根据能量，判断是否收敛
            {
                energyIsConvergence = true;
            }
            forceIsConvergence = LagrangeForceCriteria(data_MECP.functionData, ref criteria);                     //根据拉格朗日力，判断是否收敛

            if(energyIsConvergence==true && forceIsConvergence==true)
            {
                isConvergence = true;
            }

            if(mecpData.judgement.ToLower() =="energy")
            {
                isConvergence = energyIsConvergence;
            }

            //为了集中显示能量差和Lambda，给出记录。
            List<string> tmpList = new List<string>();
            tmpList.Add(data_MECP.I.ToString());                                                                                                   //步数标号
            tmpList.Add(Math.Round((data_MECP.functionData.energy1 - data_MECP.functionData.energy2), 8).ToString());                              //能量差
            tmpList.Add(Math.Round(data_MECP.functionData.Lambda, 8).ToString());                                                                  //Lambda
            tmpList.Add(Math.Round(((data_MECP.functionData.energy1 + data_MECP.functionData.energy2) / 2), 8).ToString());                        //平均能量
            tmpList.Add(Math.Round(data_MECP.criteria.maxLagrangeForce, 6).ToString());                                                            //最大Lagrange力
            tmpList.Add(Math.Round(data_MECP.criteria.RMSLagrangeForce, 6).ToString());                                                            //最大均方根Lagrange力
            data_MECP.record.Add(tmpList);

            return isConvergence;
        }

        /// <summary>
        /// 根据拉格朗日力，判断是否收敛
        /// </summary>
        /// <param name="functionData">函数数据</param>
        /// <param name="criteria">拉格朗日力的存贮位置</param>
        /// <returns></returns>
        private bool LagrangeForceCriteria(Data_MECP.FunctionData functionData, ref Data_MECP.Criteria criteria)
        {
            bool isStopping = false;
            CalculateLagrangeForce(functionData, ref criteria);                          //计算拉格朗日力
            if (criteria.maxLagrangeForce <= criteria.criteriaMax && criteria.RMSLagrangeForce <= criteria.criteriaRMS)
                isStopping = true;
            return isStopping;
        }

        /// <summary>
        /// 计算拉格朗日力
        /// </summary>
        /// <param name="functionData">函数数据</param>
        /// <param name="criteria">拉格朗日力的存贮位置</param>
        private void CalculateLagrangeForce(Data_MECP.FunctionData functionData, ref Data_MECP.Criteria criteria)
        {
            for(int i=0;i<functionData.gradient1.Length;i++)
            {
                criteria.lagrangeForce[i] = functionData.gradient1[i] * (-1) - functionData.Lambda * (functionData.gradient2[i] - functionData.gradient1[i]);
            }
            criteria.maxLagrangeForce = Tools.Statistics_Kun.MaxArray(criteria.lagrangeForce);
            criteria.maxLagrangeForce = Math.Round(criteria.maxLagrangeForce, 9);                               //保留9位有效数字
            criteria.RMSLagrangeForce = Tools.Statistics_Kun.RMSArray(criteria.lagrangeForce);
            criteria.RMSLagrangeForce = Math.Round(criteria.RMSLagrangeForce, 9);                               //保留9位有效数字
            return;
        }
    }
}
