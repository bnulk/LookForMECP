using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Data
{
    class Data_Cmd
    {
        /*
        ----------------------------------------------------  类注释  开始----------------------------------------------------
        版本：  V1.0
        作者：  刘鲲
        日期：  2017-12-26

        描述：
            * 接收控制台输入的类
        结构：
            * Cmd --- 控制台输入信息。
        ----------------------------------------------------  类注释  结束----------------------------------------------------
        */

        /// <summary>
        /// 存储命令行输入字符串的结构
        /// </summary>
        public struct CmdData
        {
            //输入文件
            public int nWord;                                   //参量的个数
            public string inputName;                            //输入文件名
            public string outputName;                           //输出文件名
            public string param;                                //计算参数
            public string directoryName;                        //当前的物理路径
        }
        public CmdData cmdData;
    }
}
