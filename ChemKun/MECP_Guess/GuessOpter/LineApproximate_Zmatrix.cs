using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemKun.Data;

namespace ChemKun.MECP_Guess.GuessOpter
{
    class LineApproximate_Zmatrix
    {
        public LineApproximate_Zmatrix(Data_Input data_Input, ref Data_MecpGuess data_MecpGuess)
        {
            double gradientRatio = (data_MecpGuess.functionData.y2 - data_MecpGuess.functionData.y1) / (data_MecpGuess.functionData.y4 - data_MecpGuess.functionData.y3);
            double yMecp = (data_MecpGuess.functionData.y1 - gradientRatio * data_MecpGuess.functionData.y3) / (1 - gradientRatio);

            double coefficient = (yMecp - data_MecpGuess.functionData.y1) / (data_MecpGuess.functionData.y2 - data_MecpGuess.functionData.y1);
            for (int i=0;i<data_MecpGuess.functionData.para.Length;i++)
            {
                data_MecpGuess.newX[i] = data_MecpGuess.functionData.x1[i] + coefficient * (data_MecpGuess.functionData.x2[i] - data_MecpGuess.functionData.x1[i]);
            }
        }
    }
}
