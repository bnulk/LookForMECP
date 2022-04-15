using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Data
{
    partial class Data_Input
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2016-6-27

        描述：
            * 接收输入数据的类
        结构：
            * MecpData ---- MECP计算相关的数据
        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */

        /// <summary>
        /// 输入文件
        /// </summary>
        public List<string> inputList;                               //Input文件字符串列表，每行一串。称为“输入列表”

        public struct KunData
        {
            public string calProgram;                                //计算所用的程序 
            public string task;                                      //计算任务，例如“MECP”“Min”“TS”
            public string cmd;                                       //计算所用程序的命令，例如"g09"
        }
        public KunData kunData;

        /// <summary>
        /// MECP任务的参数变量
        /// </summary>
        public struct MecpData
        {
            public string method;                                //计算方法
            public string coordinateType;                        //坐标类型，包括"z-matrix"和"cartesian"
            public string file1;                                 //第一个态的文件名
            public string file2;                                 //第二个态的文件名
            public string file3;                                 //第三个态的文件名
            public string calTyp;                                //计算类型
            public string scfMethod1;                            //第一个态的自洽场方法
            public string scfMethod2;                            //第二个态的自洽场方法
            public string scfTyp1;                               //第一个态的自洽场类型
            public string scfTyp2;                               //第二个态的自洽场类型
            public int cyc;                                      //最大循环次数
            public double stepSize;                              //步长
            public string guessHessian;                          //估计Hessian阵的方式，包括两种方式，默认为1、"BFGS"，即BFGS方法； 另一种是2、"Powell"，即Powell方法
            public int hessianN;                                 //计算Hessian阵的间隔步，即每隔hessianStep步计算一次力常数矩阵
            public string convergenceTyp;                        //收敛类型
            public double criterianEnergy;                       //收敛限能量
            public double criterianMax;                          //最大拉格朗日力
            public double criterianRMS;                          //均方根拉格朗日力
            public double lambda;                                //计算中所用拉格朗日参量λ
            public bool isReadFirst;                             //是否读第零步Labuta、能量、梯度和Hessian阵，第一步的构型；"true"表示读，"false"表示不读 
            public double showGradRatioCriterionN;               //最终显示梯度比的梯度阚值，10^-N
            public double showGradRatioCriterion;                //最终显示梯度比的梯度阚值
            public string judgement;                             //判据。可以是能量"energy"或者综合"global".
            public string mecpFreq;                              //mecp的振动分析

            public double sqp_tao;                               //SQP方法中的tao
            public bool check;
        }
        public MecpData mecpData;

        /// <summary>
        /// MecpGuess任务的参数变量
        /// </summary>
        public struct MecpGuessData
        {
            public string method;                                //计算方法
            public string coordinateType;                        //坐标类型
            public string scfMethod1;                            //第一个态的自洽场方法
            public string scfMethod2;                            //第二个态的自洽场方法
            public string scfTyp1;                               //第一个态的自洽场类型
            public string scfTyp2;                               //第二个态的自洽场类型
            public double criterianEnergy;                       //收敛限能量
            public int cyc;                                      //最大循环次数
        }
        public MecpGuessData mecpGuessData;

    }
}
