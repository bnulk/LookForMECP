using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemKun.MECP_Guess
{
    public class Data_MecpGuess
    {
        public string scfMethod1;
        public string scfMethod2;
        public int I;                                    //第I次计算的结果
        public bool isConvergence;                       //是否收敛

        public double[] newX;                            //新坐标数组


        public struct FunctionData
        {
            public int N;                                //原子个数
            public string coordinateType;                //坐标类型
            public double y1;
            public double y2;
            public double y3;
            public double y4;
            public double y5;
            public double y6;
            public string[] para;
            public double[] x1;
            public double[] x2;
            public double[] x3;
            public string scfTyp1;                               //第一个态的自洽场类型
            public string scfTyp2;                               //第二个态的自洽场类型
        }
        public FunctionData functionData;

        public struct Criteria
        {
            public double deltaEnergy;
        }
        public Criteria criteria;
    }
}
