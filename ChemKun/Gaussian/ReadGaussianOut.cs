using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ChemKun.Tools;

namespace ChemKun.Gaussian
{
    class ReadGaussianOut
    {
        private string FilePath;            //Out文件的物理地址
        public int N = 0;                   //分子中包含原子个数

        /// <summary>
        /// 构造函数；完成两项初始化工作。１、把GaussianOut文件的物理路径赋值给FilePath。
        /// ２、把分子中原子的个数赋值给N；
        /// </summary>
        /// <param name="filepath">GaussianOut文件的物理路径</param>
        public ReadGaussianOut(string filepath)
        {
            FilePath = filepath;      //全局变量FilePath，文件的物理路径，有初始化参量获得
            N = ObtainN();            //分子中原子个数
        }

        /// <summary>
        /// 获取原子个数
        /// </summary>
        /// <returns></returns>
        public int ObtainN()    //获取Ｎ值
        {
            int N = -1;
            int indexMark = -1;
            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);
            string str = null;
            while (OutFileStreamReader.Peek() > -1)                 //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();
                indexMark = str.IndexOf("NAtoms=");
                if(indexMark!=-1)
                {
                    str = str.Remove(0, indexMark + 7);
                    str = str.Trim();
                    indexMark = str.IndexOf(" ");
                    str = str.Substring(0, indexMark).Trim();
                    try
                    {
                        N = Convert.ToInt32(str);
                    }
                    catch
                    {
                        Console.Write("ReadGaussianOut.ObtainN() Error" + "\n");
                        Output.WriteOutput.Error.Append("ReadGaussianOut.ObtainN() Error" + "\n");
                    }
                    return N;
                }
            }
            return N;
        }

        /// <summary>
        /// 获取HF类型的能量
        /// </summary>
        /// <returns>HF类型的能量，双精度型</returns>
        public string GetArchive()
        {
            string str = null;
            string archive = "";
            StringBuilder archiveStr = new StringBuilder();
            StreamReader OutFileStreamReader = null;

            OutFileStreamReader = File.OpenText(@FilePath);
            while (OutFileStreamReader.Peek() > -1)
            {
                str = OutFileStreamReader.ReadLine();
                if (str == " Test job not archived.")
                {
                    for (; str != "";)
                    {
                        str = OutFileStreamReader.ReadLine();
                        archiveStr.Append(str.Trim());
                    }
                }
            }
            archive = archiveStr.ToString();
            if(archive=="")
            {
                Console.Write("ReadGaussianOut.GetArchive() Error" + "\n");
                Output.WriteOutput.Error.Append("ReadGaussianOut.GetArchive() Error" + "\n");
            }
            return archive;
        }


        /// <summary>
        /// 获取HF类型的能量
        /// </summary>
        /// <returns>HF类型的能量，双精度型</returns>
        public double GetHFEnergy()
        {
            double Energy = -1.0;
            int indexMark = -1;
            string str = null;

            str = GetArchive();
            indexMark = str.IndexOf("HF=");
            //Linux系统，“\”分割；Windows系统“|”分割。
            str = str.Replace("\\", "|");
            str = str.Remove(0, indexMark + 3);
            indexMark = str.IndexOf('|');
            str = str.Substring(0, indexMark);
            Energy = Convert.ToDouble(str);
            return Energy;
        }

        /// <summary>
        /// 获取CIS类型的能量
        /// </summary>
        /// <returns>CIS类型的能量，双精度型</returns>
        public double GetCISEnergy()
        {
            double Energy = -1.0;
            int indexMark = -1;
            string str = null;
            bool isReadFile = true;

            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);

            while (OutFileStreamReader.Peek() > -1 && isReadFile==true)                //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();              //读文件的一行

                //获取力
                if (str == " This state for optimization and/or second-order correction.")           //读梯度的标志
                {
                    isReadFile = false;
                    str = OutFileStreamReader.ReadLine();              //读CIS一行
                    indexMark = str.IndexOf(" Total Energy, E(CIS/TDA) =  ");
                    str = str.Remove(0, indexMark + 28);
                    Energy = Convert.ToDouble(str);
                }
            }
            return Energy;
        }

        /// <summary>
        /// 获取TD类型的能量
        /// </summary>
        /// <returns>TD类型的能量，双精度型</returns>
        public double GetTDEnergy()
        {
            double Energy = -1.0;
            int indexMark = -1;
            string str = null;
            bool isReadFile = true;

            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);

            while (OutFileStreamReader.Peek() > -1 && isReadFile == true)                //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();              //读文件的一行

                //获取力
                if (str == " This state for optimization and/or second-order correction.")           //读梯度的标志
                {
                    isReadFile = false;
                    str = OutFileStreamReader.ReadLine();              //读TD一行
                    indexMark = str.IndexOf(" Total Energy, E(TD-HF/TD-DFT) =");
                    str = str.Remove(0, indexMark + 33);
                    Energy = Convert.ToDouble(str);
                }
            }
            return Energy;
        }

        /// <summary>
        /// 获取TDA类型的能量
        /// </summary>
        /// <returns>TDA类型的能量，双精度型</returns>
        public double GetTDAEnergy()
        {
            double Energy = -1.0;
            int indexMark = -1;
            string str = null;
            bool isReadFile = true;

            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);

            while (OutFileStreamReader.Peek() > -1 && isReadFile == true)                //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();              //读文件的一行

                //获取力
                if (str == " This state for optimization and/or second-order correction.")           //读梯度的标志
                {
                    isReadFile = false;
                    str = OutFileStreamReader.ReadLine();              //读TD一行
                    indexMark = str.IndexOf(" Total Energy, E(CIS/TDA) =");
                    str = str.Remove(0, indexMark + 28);
                    Energy = Convert.ToDouble(str);
                }
            }
            return Energy;
        }

        /// <summary>
        /// 得到一行和参数对应的梯度值。 注意：本程序中所有梯度，都是-DE/DX，这个一定要当心。
        /// </summary>
        /// <returns>3*N-6个梯度值</returns>
        public string[] GetForce_Zmatrix()
        {
            string[] Force = new string[3 * N - 6];
            string[,] ForceParams = new string[3 * N - 6, 3];
            ForceParams = GetForceParams_Zmatrix();
            for (int i = 0; i < 3 * N - 6; i++)
            {
                Force[i] = ForceParams[i, 2];
            }
            return Force;
        }

        /// <summary>
        /// string［M,3］。其中M=3*N-6是变量个数，3分别是“变量名”“变量值”“梯度”
        /// </summary>
        /// <returns>string[M,3]二维数组</returns>
		public string[,] GetForceParams_Zmatrix()
        {
            string str = null;
            string[] tmpStr = new string[1000];
            string[] tmpParams = new string[7];
            string[,] ForceParams = new string[3 * N - 6, 3];
            string[] tmp3 = new string[3];                                           //
            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);

            for (int i = 0; i < 3 * N - 6; i++)           //初始化ForceParams数组，如果读不到梯度的标志，则数组不变。
            {
                for (int j = 0; j < 3; j++)
                {
                    ForceParams[i, j] = "bnulk";
                }
            }

            while (OutFileStreamReader.Peek() > -1)                //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();              //读文件的一行

                //获取力
                if (str == " From PutF, contents of force:")           //读梯度的标志
                {
                    for (int i = 0; i < (3 * N - 6); i++)
                    {
                        str = OutFileStreamReader.ReadLine();
                        //TianjinData.m_Result.Append(str.ToString() + "\n");
                        //WriteOut.Write();
                        tmpStr = str.Split(' ');
                        int k = 0;
                        foreach (string temp in tmpStr)
                        {
                            if (temp != " " && temp != "" && temp != "\r")
                            {
                                tmpParams[k] = temp;
                                k++;
                            }
                        }
                        ForceParams[i, 2] = tmpParams[1];
                    }
                }

                //获取参数名称和当前参数值
                if (str == " Variable       Old X    -DE/DX   Delta X   Delta X   Delta X     New X")           //读梯度的标志
                {
                    str = OutFileStreamReader.ReadLine();     //跳过(Linear)    (Quad)   (Total)那一行
                    for (int i = 0; i < (3 * N - 6); i++)
                    {
                        str = OutFileStreamReader.ReadLine();
                        tmp3[0] = str.Substring(0, 11);
                        tmp3[1] = str.Substring(11, 10);
                        tmp3[2] = str.Substring(21, 10);
                        ForceParams[i, 0] = tmp3[0];
                        ForceParams[i, 1] = tmp3[1];
                        //ForceParams[i,2] = tmp3[2];
                    }
                }
            }

            //把Out文件力矩阵矩阵元中的D改为E
            for (int m = 0; m < 3 * N - 6; m++)
            {
                ForceParams[m, 2] = ForceParams[m, 2].Replace('D', 'E');
            }
            //返回值
            return ForceParams;
        }

        /// <summary>
        /// 得到一行和参数对应的梯度值。 注意：本程序中所有梯度，都是-DE/DX，这个一定要当心。
        /// </summary>
        /// <returns>3*N个梯度值</returns>
        public string[] GetForce_Cartesian()
        {
            string[] Force = new string[3 * N];
            string[,] ForceParams = new string[3 * N, 3];
            ForceParams = GetForceParams_Cartesian();
            for (int i = 0; i < 3 * N; i++)
            {
                Force[i] = ForceParams[i, 2];
            }
            return Force;
        }

        /// <summary>
        /// string［M,3］。其中M=3*N是变量个数，3分别是“变量名”“变量值”“梯度”
        /// </summary>
        /// <returns>string[M,3]二维数组</returns>
		public string[,] GetForceParams_Cartesian()
        {
            string str = null;
            string[] tmpStr = new string[1000];
            string[] tmpParams = new string[7];
            string[,] ForceParams = new string[3 * N, 3];
            string[] tmp3 = new string[3];                      //
            StreamReader OutFileStreamReader = null;
            OutFileStreamReader = File.OpenText(@FilePath);

            for (int i = 0; i < 3 * N; i++)                    //初始化ForceParams数组，如果读不到梯度的标志，则数组不变。
            {
                for (int j = 0; j < 3; j++)
                {
                    ForceParams[i, j] = "bnulk";
                }
            }

            while (OutFileStreamReader.Peek() > -1)                //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();              //读文件的一行

                //获取力
                if (str == " ***** Axes restored to original set *****")           //读梯度的标志
                {
                    str = OutFileStreamReader.ReadLine();
                    str = OutFileStreamReader.ReadLine();
                    str = OutFileStreamReader.ReadLine();
                    str = OutFileStreamReader.ReadLine();
                    str = OutFileStreamReader.ReadLine();                          //跳过5行
                    for (int i = 0; i < N; i++)
                    {
                        str = OutFileStreamReader.ReadLine();
                        tmp3[0] = str.Substring(26, 12);
                        tmp3[1] = str.Substring(41, 12);
                        tmp3[2] = str.Substring(56, 12);
                        ForceParams[3 * i, 2] = tmp3[0];
                        ForceParams[3 * i + 1, 2] = tmp3[1];
                        ForceParams[3 * i + 2, 2] = tmp3[2];
                    }
                }

                //获取参数名称和当前参数值
                if (str == " Variable       Old X    -DE/DX   Delta X   Delta X   Delta X     New X")           //读梯度的标志
                {
                    str = OutFileStreamReader.ReadLine();     //跳过(Linear)    (Quad)   (Total)那一行
                    for (int i = 0; i < (3 * N); i++)
                    {
                        str = OutFileStreamReader.ReadLine();
                        tmp3[0] = str.Substring(0, 11);
                        tmp3[1] = str.Substring(11, 10);
                        tmp3[2] = str.Substring(21, 10);
                        ForceParams[i, 0] = tmp3[0];
                        ForceParams[i, 1] = tmp3[1];
                        //ForceParams[i,2] = tmp3[2];
                    }
                }
            }
            //把Out文件力矩阵矩阵元中的D改为E
            for (int m = 0; m < 3 * N; m++)
            {
                ForceParams[m, 2] = ForceParams[m, 2].Replace('D', 'E');
            }
            //返回值
            return ForceParams;
        }

        /// <summary>
        /// 得到力常数矩阵，参数的标号按Out文件中的顺序
        /// </summary>
        /// <returns>二维数组，力常数矩阵string[3*N-6,3*N-6]</returns>
        //public string[,] GetForceConstant()
        public string[,] GetForceConstant_Zmatrix()
        {
            //数据准备
            string str = "";         //临时存放每一行数据
            string[] temp_Data = new string[1000];   //临时存放数据行被分割的部分
            string[] Data = new string[6];           //把每一行数据按空格分成六份,每一份为一个力常数数据
            //初始化每行数据Data
            for (int i = 0; i < 6; i++)
            {
                Data[i] = "ForceConstant";
            }
            int Block = 0;                            //力常数矩阵在Out文件中被分成的块数
            int N = ObtainN();                        //获取原子个数N值
            Block = Convert.ToInt32(Math.Floor((3 * Convert.ToDouble(N) - 6) / 5) + 1);         //力常数矩阵的块数
            string[,] ForceConstant = new string[3 * N - 6, 3 * N - 6];        //定义力常数矩阵数组
            //初始化力常数矩阵
            for (int i = 0; i < 3 * N - 6; i++)
            {
                for (int j = 0; j < 3 * N - 6; j++)
                {
                    ForceConstant[i, j] = "bnulk";
                }
            }

            //正式开始操作
            StreamReader OutFileStreamReader = null;            //生成一个StreamReader的实例OutFileStreamReader；用读方式打开GaussianOut文件
            OutFileStreamReader = File.OpenText(@FilePath);     //用OutFileStreamReader打开指定的GaussianOut文件FilePath，使用了System.IO.File名称空间的OpenText方法

            while (OutFileStreamReader.Peek() > -1)               //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();             //读文件的一行
                if (str == " Force constants in internal coordinates: ")           //读梯度的标志
                {
                    //跳过没有数据的一行
                    str = OutFileStreamReader.ReadLine();

                    for (int i = 0; i < Block; i++)              //按块读数据
                    {
                        for (int m = 5 * i; m < 3 * N - 6; m++)      //读行
                        {
                            //读文本中每一行数据
                            str = OutFileStreamReader.ReadLine();
                            temp_Data = str.Split(' ');
                            for (int j = 0, k = 0; j < temp_Data.Length; j++)
                            {
                                if (temp_Data[j] != "")
                                {
                                    Data[k] = temp_Data[j];
                                    k++;
                                }
                            }
                            if (i * 5 + 5 <= 3 * N - 6)                          //判断是否为最后一个力常数块
                            //不是最后一个力常数块
                            {
                                for (int n = 5 * i; n < 5 + 5 * i; n++) //读列
                                {
                                    if (n <= m)
                                    {
                                        ForceConstant[m, n] = Data[n - 5 * i + 1];
                                    }
                                }
                            }
                            else
                            //最后一个力常数块
                            {
                                for (int n = 5 * i; n < 3 * N - 6; n++) //读列
                                {
                                    if (n <= m)
                                    {
                                        ForceConstant[m, n] = Data[n - 5 * i + 1];
                                    }
                                }
                            }
                        }

                        str = OutFileStreamReader.ReadLine();     //跳过Block之间的构型参数
                    }
                    for (int m = 0; m < 3 * N - 6; m++)
                    {
                        for (int n = 0; n < 3 * N - 6; n++)
                        {
                            if (m < n)
                            {
                                ForceConstant[m, n] = ForceConstant[n, m];
                            }
                        }
                    }
                    //把Out文件力常数矩阵矩阵元中的D改为E
                    for (int m = 0; m < 3 * N - 6; m++)
                    {
                        for (int n = 0; n < 3 * N - 6; n++)
                        {
                            ForceConstant[m, n] = ForceConstant[m, n].Replace('D', 'E');
                        }
                    }
                }
            }
            return ForceConstant;
        }

        /// <summary>
        /// 得到力常数矩阵，参数的标号按Out文件中的顺序
        /// </summary>
        /// <returns>二维数组，力常数矩阵string[3*N,3*N]</returns>
        public string[,] GetForceConstant_Cartesian()
        {
            //数据准备
            string str = "";         //临时存放每一行数据
            string[] temp_Data = new string[1000];   //临时存放数据行被分割的部分
            string[] Data = new string[6];           //把每一行数据按空格分成六份,每一份为一个力常数数据
            //初始化每行数据Data
            for (int i = 0; i < 6; i++)
            {
                Data[i] = "ForceConstant";
            }
            int Block = 0;                            //力常数矩阵在Out文件中被分成的块数
            int N = ObtainN();                        //获取原子个数N值
            Block = Convert.ToInt32(Math.Floor((3 * Convert.ToDouble(N)) / 5) + 1);         //力常数矩阵的块数
            string[,] ForceConstant = new string[3 * N, 3 * N];        //定义力常数矩阵数组
            //初始化力常数矩阵
            for (int i = 0; i < 3 * N; i++)
            {
                for (int j = 0; j < 3 * N; j++)
                {
                    ForceConstant[i, j] = "bnulk";
                }
            }

            //正式开始操作
            StreamReader OutFileStreamReader = null;            //生成一个StreamReader的实例OutFileStreamReader；用读方式打开GaussianOut文件
            OutFileStreamReader = File.OpenText(@FilePath);     //用OutFileStreamReader打开指定的GaussianOut文件FilePath，使用了System.IO.File名称空间的OpenText方法

            while (OutFileStreamReader.Peek() > -1)               //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();             //读文件的一行
                if (str == " Force constants in Cartesian coordinates: ")           //读梯度的标志
                {
                    //跳过没有数据的一行
                    str = OutFileStreamReader.ReadLine();

                    for (int i = 0; i < Block; i++)              //按块读数据
                    {
                        for (int m = 5 * i; m < 3 * N; m++)      //读行
                        {
                            //读文本中每一行数据
                            str = OutFileStreamReader.ReadLine();
                            temp_Data = str.Split(' ');
                            for (int j = 0, k = 0; j < temp_Data.Length; j++)
                            {
                                if (temp_Data[j] != "")
                                {
                                    Data[k] = temp_Data[j];
                                    k++;
                                }
                            }
                            if (i * 5 + 5 <= 3 * N)                          //判断是否为最后一个力常数块
                            //不是最后一个力常数块
                            {
                                for (int n = 5 * i; n < 5 + 5 * i; n++) //读列
                                {
                                    if (n <= m)
                                    {
                                        ForceConstant[m, n] = Data[n - 5 * i + 1];
                                    }
                                }
                            }
                            else
                            //最后一个力常数块
                            {
                                for (int n = 5 * i; n < 3 * N; n++) //读列
                                {
                                    if (n <= m)
                                    {
                                        ForceConstant[m, n] = Data[n - 5 * i + 1];
                                    }
                                }
                            }
                        }

                        str = OutFileStreamReader.ReadLine();     //跳过Block之间的构型参数
                    }
                    for (int m = 0; m < 3 * N; m++)
                    {
                        for (int n = 0; n < 3 * N; n++)
                        {
                            if (m < n)
                            {
                                ForceConstant[m, n] = ForceConstant[n, m];
                            }
                        }
                    }
                    //把Out文件力常数矩阵矩阵元中的D改为E
                    for (int m = 0; m < 3 * N; m++)
                    {
                        for (int n = 0; n < 3 * N; n++)
                        {
                            ForceConstant[m, n] = ForceConstant[m, n].Replace('D', 'E');
                        }
                    }
                }
            }
            return ForceConstant;
        }

        /// <summary>
        /// 读输入取向坐标
        /// </summary>
        /// <param name="atomicNumber">原子序数</param>
        /// <param name="atomicType">原子类型</param>
        /// <param name="coordinates_Angstroms">坐标</param>
        public void ReadInputOrientation(out int[] atomicNumber, out int[] atomicType, out double[,] coordinates_Angstroms)
        {
            int dim = 3 * N;
            atomicNumber = new int[N];
            atomicType = new int[N];
            coordinates_Angstroms = new double[dim, 3];
            string[] tmpWords = new string[6];

            //正式开始操作
            StreamReader OutFileStreamReader = null;            //生成一个StreamReader的实例OutFileStreamReader；用读方式打开GaussianOut文件
            OutFileStreamReader = File.OpenText(@FilePath);     //用OutFileStreamReader打开指定的GaussianOut文件FilePath，使用了System.IO.File名称空间的OpenText方法
            string str;

            //定位
            str = OutFileStreamReader.ReadLine();             //读文件的一行
            while (str.Trim() != "Input orientation:" && OutFileStreamReader.Peek() > -1)               //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();
            }
            for(int i=0;i<4;i++)
            {
                str = OutFileStreamReader.ReadLine();
            }

            //读取数据
            for(int i=0;i<N;i++)
            {
                str = OutFileStreamReader.ReadLine();
                tmpWords = Text_Kun.GetWords(str);
                atomicNumber[i] = Convert.ToInt32(tmpWords[1]);
                atomicType[i] = Convert.ToInt32(tmpWords[2]);
                coordinates_Angstroms[i, 0] = Convert.ToDouble(tmpWords[3]);
                coordinates_Angstroms[i, 1] = Convert.ToDouble(tmpWords[4]);
                coordinates_Angstroms[i, 2] = Convert.ToDouble(tmpWords[5]);
            }
            
            return;
        }

        public void ReadInputOrientationForce(out double[] force)
        {
            int dim = 3 * N;
            force = new double[dim];
            string[] tmpWords = new string[5];

            //正式开始操作
            StreamReader OutFileStreamReader = null;            //生成一个StreamReader的实例OutFileStreamReader；用读方式打开GaussianOut文件
            OutFileStreamReader = File.OpenText(@FilePath);     //用OutFileStreamReader打开指定的GaussianOut文件FilePath，使用了System.IO.File名称空间的OpenText方法
            string str;

            //定位
            str = OutFileStreamReader.ReadLine();             //读文件的一行
            while (str.Trim() != "***** Axes restored to original set *****" && OutFileStreamReader.Peek() > -1)               //应用一个while循环，条件是文件不结束
            {
                str = OutFileStreamReader.ReadLine();
            }
            for (int i = 0; i < 5; i++)
            {
                str = OutFileStreamReader.ReadLine();
            }

            //读取数据
            for (int i = 0; i < N; i++)
            {
                str = OutFileStreamReader.ReadLine();
                tmpWords = Text_Kun.GetWords(str);
                force[3 * i] = Convert.ToDouble(tmpWords[2]);
                force[3 * i + 1] = Convert.ToDouble(tmpWords[3]);
                force[3 * i + 2] = Convert.ToDouble(tmpWords[4]);
            }

            return;
        }

        /// <summary>
        /// 读L703后的力常数矩阵，即输入坐标下的力常数矩阵
        /// </summary>
        /// <param name="forceConstant">力常数矩阵</param>
        public void ReadL703Hessian(out double[,] forceConstant)
        {
            //数据准备
            string str = "";         //临时存放每一行数据
            string[] temp_Data = new string[1000];   //临时存放数据行被分割的部分
            string[] Data = new string[6];           //把每一行数据按空格分成六份,每一份为一个力常数数据
            //初始化每行数据Data
            for (int i = 0; i < 6; i++)
            {
                Data[i] = "ForceConstant";
            }
            int Block = 0;                            //力常数矩阵在Out文件中被分成的块数
            int N = ObtainN();                        //获取原子个数N值
            int dim = 3 * N;
            Block = Convert.ToInt32(Math.Floor((3 * Convert.ToDouble(N)) / 5) + 1);         //力常数矩阵的块数
            string[,] strForceConstant = new string[dim, dim];        //定义力常数矩阵数组
            forceConstant = new double[dim, dim];
            //初始化力常数矩阵
            for (int i = 0; i < 3 * N; i++)
            {
                for (int j = 0; j < 3 * N; j++)
                {
                    strForceConstant[i, j] = "bnulk";
                }
            }

            //正式开始操作
            StreamReader OutFileStreamReader = null;            //生成一个StreamReader的实例OutFileStreamReader；用读方式打开GaussianOut文件
            OutFileStreamReader = File.OpenText(@FilePath);     //用OutFileStreamReader打开指定的GaussianOut文件FilePath，使用了System.IO.File名称空间的OpenText方法

            //定位
            while (str.Trim() != "Hessian after L703:" && OutFileStreamReader.Peek() > -1)
            {
                str = OutFileStreamReader.ReadLine();             //读文件的一行
            }

            //读取数据
            str = OutFileStreamReader.ReadLine();                        //跳过没有数据的一行
            for (int i = 0; i < Block; i++)              //按块读数据
            {
                for (int m = 5 * i; m < dim; m++)      //读行
                {
                    //读文本中每一行数据
                    str = OutFileStreamReader.ReadLine();
                    temp_Data = str.Split(' ');
                    for (int j = 0, k = 0; j < temp_Data.Length; j++)
                    {
                        if (temp_Data[j] != "")
                        {
                            Data[k] = temp_Data[j];
                            k++;
                        }
                    }
                    if (i * 5 + 5 <= dim)                          //判断是否为最后一个力常数块
                                                                     //不是最后一个力常数块
                    {
                        for (int n = 5 * i; n < 5 + 5 * i; n++) //读列
                        {
                            if (n <= m)
                            {
                                strForceConstant[m, n] = Data[n - 5 * i + 1];
                            }
                        }
                    }
                    else
                    //最后一个力常数块
                    {
                        for (int n = 5 * i; n < dim; n++) //读列
                        {
                            if (n <= m)
                            {
                                strForceConstant[m, n] = Data[n - 5 * i + 1];
                            }
                        }
                    }
                }
                str = OutFileStreamReader.ReadLine();     //跳过Block之间的构型参数
            }

            //补齐力常数矩阵
            for (int m = 0; m < dim; m++)
            {
                for (int n = 0; n < dim; n++)
                {
                    if (m < n)
                    {
                        strForceConstant[m, n] = strForceConstant[n, m];
                    }
                }
            }
            //把Out文件力常数矩阵矩阵元中的D改为E，然后把字符串转为双精度的数
            for (int m = 0; m < dim; m++)
            {
                for (int n = 0; n < dim; n++)
                {
                    strForceConstant[m, n] = strForceConstant[m, n].Replace('D', 'E');
                    forceConstant[m, n] = Convert.ToDouble(strForceConstant[m, n]);
                }
            }
            
            return;
        }
    }
}
