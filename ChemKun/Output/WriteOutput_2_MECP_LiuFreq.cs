using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.LinearAlgebra;

namespace ChemKun.Output
{
    static partial class WriteOutput
    {
        public static void VibrationalAnalysis(int[] atomicNumbers, BnulkVec frequences, BnulkMatrix vibrationalMode)
        {
            m_Result.Clear();
            int numberOfVibration = frequences.dim;
            int N = vibrationalMode.row / 3;
            int numberOfSegment = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(numberOfVibration) / 3));

            m_Result.Append("\n");
            m_Result.Append("bnulk@foxmail.com-MECP LiuFreq" + "\n");
            m_Result.Append("*********************************************" + "\n\n");


            for (int i = 0; i < numberOfSegment; i++)
            {
                if (numberOfVibration > 3 * i + 3)
                {
                    m_Result.Append((3 * i + 1).ToString().PadLeft(23) + (3 * i + 2).ToString().PadLeft(23) + (3 * i + 3).ToString().PadLeft(23) + "\n");
                    m_Result.Append(" " + "Frequencies --" + frequences[3 * i].ToString("0.0000").PadLeft(12)
                        + frequences[3 * i + 1].ToString("0.0000").PadLeft(23) + frequences[3 * i + 2].ToString("0.0000").PadLeft(23) + "\n");
                    m_Result.Append("  Atom  AN      X      Y      Z        X      Y      Z        X      Y      Z" + "\n");
                    for (int j = 0; j < N; j++)
                    {
                        m_Result.Append(" " + (j + 1).ToString().PadLeft(5) + atomicNumbers[j].ToString().PadLeft(4) + "  ");
                        m_Result.Append(vibrationalMode[3 * j, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i].ToString("0.00").PadLeft(7)
                            + "  " + vibrationalMode[3 * j, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i + 1].ToString("0.00").PadLeft(7)
                            + "  " + vibrationalMode[3 * j, 3 * i + 2].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i + 2].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i + 2].ToString("0.00").PadLeft(7));
                        m_Result.Append("\n");
                    }
                }
                if (numberOfVibration == 3 * i + 3)
                {
                    m_Result.Append((3 * i + 1).ToString().PadLeft(23) + (3 * i + 2).ToString().PadLeft(23) + (3 * i + 3).ToString().PadLeft(23) + "\n");
                    m_Result.Append(" " + "Frequencies --" + frequences[3 * i].ToString("0.0000").PadLeft(12)
                        + frequences[3 * i + 1].ToString("0.0000").PadLeft(23) + frequences[3 * i + 2].ToString("0.0000").PadLeft(23) + "\n");
                    m_Result.Append("  Atom  AN      X      Y      Z        X      Y      Z        X      Y      Z" + "\n");
                    for (int j = 0; j < N; j++)
                    {
                        m_Result.Append(" " + (j + 1).ToString().PadLeft(5) + atomicNumbers[j].ToString().PadLeft(4) + "  ");
                        m_Result.Append(vibrationalMode[3 * j, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i].ToString("0.00").PadLeft(7)
                            + "  " + vibrationalMode[3 * j, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i + 1].ToString("0.00").PadLeft(7)
                            + "  " + vibrationalMode[3 * j, 3 * i + 2].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i + 2].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i + 2].ToString("0.00").PadLeft(7));
                        m_Result.Append("\n");
                    }
                }
                if (numberOfVibration == 3 * i + 2)
                {
                    m_Result.Append((3 * i + 1).ToString().PadLeft(23) + (3 * i + 2).ToString().PadLeft(23) + "\n");
                    m_Result.Append(" " + "Frequencies --" + frequences[3 * i].ToString("0.0000").PadLeft(12)
                        + frequences[3 * i + 1].ToString("0.0000").PadLeft(23) + "\n");
                    m_Result.Append("  Atom  AN      X      Y      Z        X      Y      Z" + "\n");
                    for (int j = 0; j < N; j++)
                    {
                        m_Result.Append(" " + (j + 1).ToString().PadLeft(5) + atomicNumbers[j].ToString().PadLeft(4) + "  ");
                        m_Result.Append(vibrationalMode[3 * j, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i].ToString("0.00").PadLeft(7)
                            + "  " + vibrationalMode[3 * j, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i + 1].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i + 1].ToString("0.00").PadLeft(7));
                        m_Result.Append("\n");
                    }
                }
                if (numberOfVibration == 3 * i + 1)
                {
                    m_Result.Append((3 * i + 1).ToString().PadLeft(23) + "\n");
                    m_Result.Append(" " + "Frequencies --" + frequences[3 * i].ToString("0.0000").PadLeft(12)
                        + frequences[3 * i + 1].ToString("0.0000").PadLeft(23) + "\n");
                    m_Result.Append("  Atom  AN      X      Y      Z" + "\n");
                    for (int j = 0; j < N; j++)
                    {
                        m_Result.Append(" " + (j + 1).ToString().PadLeft(5) + atomicNumbers[j].ToString().PadLeft(4) + "  ");
                        m_Result.Append(vibrationalMode[3 * j, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 1, 3 * i].ToString("0.00").PadLeft(7) + vibrationalMode[3 * j + 2, 3 * i].ToString("0.00").PadLeft(7));
                        m_Result.Append("\n");
                    }
                }
            }
            Write();
            m_Result.Clear();
            return;
        }

        public static void WriteLiuFreq(bool isConvergence, bool isRealMECP)
        {
            m_Result.Clear();
            //输出结果内容标志
            //m_Result.Append("\n");
            //m_Result.Append("bnulk@foxmail.com-MECP LiuFreq" + "\n");
            //m_Result.Append("*********************************************" + "\n\n");

            if (isConvergence == false)
            {
                m_Result.Append("*****************************************************************" + "\n\n");
                m_Result.Append("Unfortunately, the KKT point is not found." + "\n\n");
                m_Result.Append("*****************************************************************" + "\n\n");
            }
            else
            {
                if (isRealMECP == false)
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
