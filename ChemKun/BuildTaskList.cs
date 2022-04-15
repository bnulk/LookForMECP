using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.MECP;


namespace ChemKun
{
    partial class Program
    {
        public static void BuildTaskList(string task, Data_Input data_Input)
        {
            switch(task.ToLower())
            {
                case "mecp":
                    RunMECP runMECP = new RunMECP(data_Input);
                    break;
                case "mecpguess":
                    MECP_Guess.RunMecpGuess runMecpGuess = new MECP_Guess.RunMecpGuess(data_Input);
                    break;
                default:
                    break;
            }
        }
    }
}
