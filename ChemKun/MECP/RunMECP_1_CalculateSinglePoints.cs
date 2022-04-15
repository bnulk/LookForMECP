using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using ChemKun.Data;


namespace ChemKun.MECP
{
    partial class RunMECP
    {
        private void CalculateSinglePoints(Data_Input data_Input, int I)
        {
            switch (data_Input.kunData.calProgram)
            {
                case "gaussian":
                    CalculateSinglePoints_Gaussian(data_Input, I);
                    break;
                default:
                    break;
            }
            return;
        }

        private void CalculateSinglePoints_Gaussian(Data_Input data_Input, int I)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //改变当前目录
            if(OS.OS.osClass == "windows")
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
                RunGaussian09.StartInfo.Arguments = "State1_" + I.ToString() + ".gjf" + " " + "State1_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
                //计算第二个点
                RunGaussian09.StartInfo.FileName = data_Input.kunData.cmd;
                RunGaussian09.StartInfo.Arguments = "State2_" + I.ToString() + ".gjf" + " " + "State2_" + I.ToString() + ".out";
                RunGaussian09.EnableRaisingEvents = true;
                RunGaussian09.Start();
                RunGaussian09.WaitForExit();
                RunGaussian09.Close();
            }
            catch
            {
                Console.WriteLine("MECP.RunMECP_1_CalculateSinglePoints.Gaussian Error." + "\n");
                Output.WriteOutput.Error.Append("MECP.RunMECP_1_CalculateSinglePoints.Gaussian Error." + "\n");
            }
            //回到原始目录
            Directory.SetCurrentDirectory(currentDirectory);
            return;
        }
    }
}
