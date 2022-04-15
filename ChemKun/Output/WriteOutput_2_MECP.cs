using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        /// <summary>
        /// MECP初始信息
        /// </summary>
        /// <param name="data_Input">输入数据</param>
        /// <param name="data_MECP">MECP数据</param>
        public static void WriteMECP(Data_Input data_Input, Data_MECP data_MECP)
        {
            if (data_MECP.I == 0)
            {
                m_Result.Clear();
                //输入文件内容标志
                m_Result.Append("\n");
                m_Result.Append("bnulk@foxmail.com-MECP Info" + "\n");
                m_Result.Append("*********************************************" + "\n\n");
                //输入文件部分
                m_Result.Append("MECP method is " + data_Input.mecpData.method.ToString() + "\n");
            }
            else
            {
                m_Result.Clear();
            }

            WriteMECPData(data_MECP);

            WriteOutput.Write();
            return;
        }

        /// <summary>
        /// 计算过程中的输出
        /// </summary>
        /// <param name="data_MECP">MECP数据</param>
        public static void WriteMECPData(Data_MECP data_MECP)
        {
            m_Result.Append("##########     I is:" + data_MECP.I.ToString() + "     ##########" + "\n");
            m_Result.Append("The Energy of the First State is:" + data_MECP.functionData.energy1.ToString() + "\n");
            m_Result.Append("The Energy of the Second State is:" + data_MECP.functionData.energy2.ToString() + "\n");
            m_Result.Append("The Energy Difference between the Two States is:" + (data_MECP.functionData.energy1 - data_MECP.functionData.energy2).ToString() + "\n");
            m_Result.Append("Lambda is:" + data_MECP.functionData.Lambda.ToString() + "\n");
            m_Result.Append("Stepsize is:" + data_MECP.stepSize.ToString() + "\n");
            //集中显示重要结果
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

            /*
            //输出梯度和力常数
            for (int i=0;i< data_MECP.functionData.gradient1.Length; i++)
            {
                m_Result.Append(data_MECP.functionData.gradient1[i].ToString().PadLeft(20));
                if ((i+1)%3==0)
                    m_Result.Append("\n");
            }

            for (int i = 0; i < data_MECP.functionData.gradient1.Length; i++)
            {
                for (int j = 0; j < data_MECP.functionData.gradient1.Length; j++)
                {
                    if(i>=j)
                    {
                        m_Result.Append(data_MECP.functionData.hessian1[i, j].ToString().PadLeft(20));
                    }
                    if ((j+1)%5 == 0)
                        m_Result.Append("\n");
                }
                m_Result.Append("\n");
            }
            */
            string deltaEnergyConverged = "No";
            if (Math.Abs(data_MECP.criteria.deltaEnergy) < data_MECP.criteria.criteriaEnergy)
                deltaEnergyConverged = "Yes";
            string maxConverged = "No";
            if (data_MECP.criteria.maxLagrangeForce < data_MECP.criteria.criteriaMax)
                maxConverged = "Yes";
            string RMSConverged = "No";
            if (data_MECP.criteria.RMSLagrangeForce < data_MECP.criteria.criteriaRMS)
                RMSConverged = "Yes";
            m_Result.Append("         Item    ".PadRight(25) + "   Value ".PadRight(15) + "  Threshold".PadRight(15) + "     Converged?".PadRight(15) + "\n");
            m_Result.Append(" Delta Energy".PadRight(25) + data_MECP.criteria.deltaEnergy.ToString("E5").PadRight(15) + ("  " + data_MECP.criteria.criteriaEnergy.ToString()).PadRight(15) + "         " + deltaEnergyConverged.PadRight(15) + "\n");
            m_Result.Append(" Maximum KKT Force".PadRight(25) + data_MECP.criteria.maxLagrangeForce.ToString("0.000000").PadRight(15) + ("  " + data_MECP.criteria.criteriaMax.ToString()).PadRight(15) + "         " + maxConverged.PadRight(15) + "\n");
            m_Result.Append(" RMS KKT Force".PadRight(25) + data_MECP.criteria.RMSLagrangeForce.ToString("0.000000").PadRight(15) + ("  " + data_MECP.criteria.criteriaRMS.ToString()).PadRight(15) + "         " + RMSConverged.PadRight(15) + "\n");
            return;
        }

        public static void WriteMECPResult(Data_MECP data_MECP)
        {
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
            return;
        }

        public static void WriteMECPFreq(Data_MECP data_MECP)
        {
            int cycle = 0;                              //循环次数
            m_Result.Clear();
            //输出结果内容标志
            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-MECP Freq" + "\n");
            m_Result.Append("*********************************************" + "\n\n");
            //输出结果内容
            /*
            if (data_MECP.functionData.coordinateType == "cartesian")
            {
                m_Result.Append("Freq calculation need z-matrix" + "\n");
            }
            */

            /*
            //原子个数小于10时，才显示borderedHessian
            if (data_MECP.functionData.N < 10)
            {
                m_Result.Append("oldBorderedHessian:" + "\n");
                for (int i = 0; i < data_MECP.freq.oldBorderedHessian.GetLength(0); i++)
                {
                    for (int j = 0; j < data_MECP.freq.oldBorderedHessian.GetLength(0); j++)
                    {
                        m_Result.Append(data_MECP.freq.oldBorderedHessian[i, j].ToString("0.0000000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }
            }
            m_Result.Append("\n");
            //原子个数小于10时，才显示borderedHessian
            if (data_MECP.functionData.N < 10)
            {
                m_Result.Append("borderedHessian:" + "\n");
                for (int i = 0; i < data_MECP.freq.borderedHessian.GetLength(0); i++)
                {
                    for (int j = 0; j < data_MECP.freq.borderedHessian.GetLength(0); j++)
                    {
                        m_Result.Append(data_MECP.freq.borderedHessian[i, j].ToString("0.0000000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }
            }
            m_Result.Append("\n");

            m_Result.Append("detDiagonal:" + "\n");
            cycle = data_MECP.freq.diagonalBorderedHessian.Length;
            for (int i = 0; i < cycle; i++)
            {
                if (Math.Abs(data_MECP.freq.diagonalBorderedHessian[i]) > 0.000001)
                {
                    m_Result.Append(data_MECP.freq.diagonalBorderedHessian[i].ToString("0.00000").PadLeft(20));
                }
                else
                {
                    m_Result.Append("tooSmallValue".PadLeft(20));
                }

                if (Math.IEEERemainder(i + 1, 5) == 0)
                    m_Result.Append("\n");
            }
            m_Result.Append("\n\n");

            m_Result.Append("Luenberger Rule:" + "\n");
            cycle = data_MECP.freq.dim - 2;
            for (int i = 0; i < cycle; i++)
            {
                if (Math.Abs(data_MECP.freq.diagonalBorderedHessian[i + 2]) > 0.000001)
                {
                    m_Result.Append(data_MECP.freq.diagonalBorderedHessian[i + 2].ToString("0.00000").PadLeft(20));
                }
                else
                {
                    m_Result.Append("tooSmallValue".PadLeft(20));
                }

                if (Math.IEEERemainder(i + 1, 5) == 0)
                    m_Result.Append("\n");
            }
            m_Result.Append("\n\n");

            m_Result.Append("eigenValue:" + "\n");
            cycle = data_MECP.freq.diagonalBorderedHessian.Length;
            for (int i = 0; i < cycle; i++)
            {
                if (Math.Abs(data_MECP.freq.diagonalBorderedHessian[i]) > 0.000001)
                {
                    m_Result.Append(("eigenValue" + i.ToString() + " = " + data_MECP.freq.diagonalBorderedHessian[i].ToString("0.000000")).PadLeft(40));
                }
                else
                {
                    m_Result.Append("          tooSmallValue".PadLeft(40));
                }

                if (Math.IEEERemainder(i + 1, 5) == 0)
                    m_Result.Append("\n");
            }
            m_Result.Append("\n\n");
            


            m_Result.Append("eigenVector:" + "\n");
            for (int i = 0; i < data_MECP.freq.dim; i += 5)
            {
                for (int j = i; j < i + 5 && j < data_MECP.freq.dim; j++)
                {
                    m_Result.Append((" eigenValue" + j.ToString()).PadLeft(20));
                }
                m_Result.Append("\n");

                for (int k = 0; k < data_MECP.freq.dim; k++)
                {
                    for (int j = i; j < i + 5 && j < data_MECP.freq.dim; j++)
                    {
                        m_Result.Append(data_MECP.freq.eigenVecBorderedHessian[j][k].ToString("0.000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }

                m_Result.Append("\n");
            }
            m_Result.Append("\n\n");
            */


            //原子个数小于10时，才显示borderedHessian
            if (data_MECP.mecpFreq =="simple" && data_MECP.functionData.N < 10)
            {
                m_Result.Append("L Matrix:" + "\n");
                cycle = data_MECP.freq.dim - 1;
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 0; j < cycle; j++)
                    {
                        m_Result.Append(data_MECP.freq.L[i, j].ToString("0.000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }
                m_Result.Append("\n\n");

                m_Result.Append("E Matrix:" + "\n");
                cycle = data_MECP.freq.dim - 1;
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 0; j < cycle - 1; j++)
                    {
                        m_Result.Append(data_MECP.freq.E[i, j].ToString("0.000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }
                m_Result.Append("\n\n");

                m_Result.Append("EtLE Matrix:" + "\n");
                cycle = data_MECP.freq.dim - 2;
                for (int i = 0; i < cycle; i++)
                {
                    for (int j = 0; j < cycle; j++)
                    {
                        m_Result.Append(data_MECP.freq.EtLE[i, j].ToString("0.000000").PadLeft(20));
                    }
                    m_Result.Append("\n");
                }
                m_Result.Append("\n\n");
            }
            

            m_Result.Append("eigenValue:" + "\n");
            cycle = data_MECP.freq.dim - 2;
            for (int i = 0; i < cycle; i++)
            {
                if (Math.Abs(data_MECP.freq.eigenValueEtLE[i]) > 0.000001)
                {
                    m_Result.Append(("eigenValue" + i.ToString() + "=" + data_MECP.freq.eigenValueEtLE[i].ToString("0.000000")).PadLeft(40));
                }
                else
                {
                    m_Result.Append("          tooSmallValue".PadLeft(40));
                }

                if (Math.IEEERemainder(i + 1, 5) == 0)
                    m_Result.Append("\n");
            }
            m_Result.Append("\n\n");

            if (data_MECP.functionData.N < 10)
            {
                m_Result.Append("eigenVector:" + "\n");
                for (int i = 0; i < cycle; i += 5)
                {
                    for (int j = i; j < i + 5 && j < cycle; j++)
                    {
                        m_Result.Append(("  eigenValue" + j.ToString()).PadLeft(20));
                    }
                    m_Result.Append("\n");

                    for (int k = 0; k < cycle; k++)
                    {
                        for (int j = i; j < i + 5 && j < cycle; j++)
                        {
                            m_Result.Append(data_MECP.freq.eigenVecEtLE[j][k].ToString("0.000000").PadLeft(20));
                        }
                        m_Result.Append("\n");
                    }

                    m_Result.Append("\n");
                }
                m_Result.Append("\n\n");
            }

            if (data_MECP.isConvergence == false)
            {
                m_Result.Append("*****************************************************************" + "\n\n");
                m_Result.Append("Unfortunately, the KKT point is not found." + "\n\n");
                m_Result.Append("*****************************************************************" + "\n\n");
            }
            else
            {
                if (data_MECP.freq.isRealMECP == false)
                {
                    m_Result.Append("*****************************************************************" + "\n\n");
                    m_Result.Append("Unfortunately, the KKT point is not a real minimum." + "\n\n");
                    m_Result.Append("*****************************************************************" + "\n\n");
                }
                else
                {
                    m_Result.Append("*****************************************************************" + "\n\n");
                    m_Result.Append("Congratulations! the KKT point is a real minimum." + "\n\n");
                    m_Result.Append("*****************************************************************" + "\n\n");
                }
            }

            WriteOutput.Write();
            return;
        }

    }
}
