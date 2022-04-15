using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using ChemKun.Data;

namespace ChemKun.MECP_Guess
{
    partial class RunMecpGuess
    {
        private void CalculateSinglePoints(Data_Input data_Input, int I)
        {
            switch (data_Input.kunData.calProgram)
            {
                case "gaussian":
                    if(I==0)
                    {
                        FirstCalculateSinglePoints_Gaussian(data_Input, I);
                    }
                    else
                    {
                        CalculateSinglePoints_Gaussian(data_Input, I);
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        private void FirstCalculateSinglePoints_Gaussian(Data_Input data_Input, int I)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //改变当前目录
            if (OS.OS.osClass == "windows")
            {
                Directory.SetCurrentDirectory(currentDirectory + "\\tmp");
            }
            else
            {
                Directory.SetCurrentDirectory(currentDirectory + "//tmp");
            }
            //运行高斯
            try
            {
                Process RunGaussian09 = new Process();
                //计算第一个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "1_" + I.ToString() + ".gjf" + " " + "1_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第二个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "2_" + I.ToString() + ".gjf" + " " + "2_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第三个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "3_" + I.ToString() + ".gjf" + " " + "3_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第四个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "4_" + I.ToString() + ".gjf" + " " + "4_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
            }
            catch
            {
                Console.WriteLine("MECP_Guess.RunMecpGuess_1_CalculateSinglePoints.Gaussian Error." + "\n");
                Output.WriteOutput.Error.Append("MECP_Guess.RunMecpGuess_1_CalculateSinglePoints.Gaussian Error." + "\n");
            }
            //回到原始目录
            Directory.SetCurrentDirectory(currentDirectory);
            return;
        }

        private void CalculateSinglePoints_Gaussian(Data_Input data_Input, int I)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //改变当前目录
            if (OS.OS.osClass == "windows")
            {
                Directory.SetCurrentDirectory(currentDirectory + "\\tmp");
            }
            else
            {
                Directory.SetCurrentDirectory(currentDirectory + "//tmp");
            }
            //运行高斯
            try
            {
                Process RunGaussian09 = new Process();
                //计算第五个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "5_" + I.ToString() + ".gjf" + " " + "5_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第六个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "6_" + I.ToString() + ".gjf" + " " + "6_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
            }
            catch
            {
                Console.WriteLine("MECP_Guess.RunMecpGuess_1_CalculateSinglePoints.Gaussian Error." + "\n");
                Output.WriteOutput.Error.Append("MECP_Guess.RunMecpGuess_1_CalculateSinglePoints.Gaussian Error." + "\n");
            }
            //回到原始目录
            Directory.SetCurrentDirectory(currentDirectory);
            return;
        }


    }
}
