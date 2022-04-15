using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.OS
{
    public partial class OS
    {
        //全局变量
        public static string osClass = "linux";        //操作系统类别

        public OS()
        {
            ObtianOsClass();
        }

        /// <summary>
        /// 获取操作系统信息。osInfo是"linux",或者"windows".
        /// </summary>
        public static string ObtianOsClass()
        {
            string tmpOsInfo = "linux";
            string str;
            int indexMark;
            str = Environment.OSVersion.VersionString.ToLower();
            indexMark = str.IndexOf("win");
            if(indexMark != -1)
            {
                tmpOsInfo = "windows";
            }
            return tmpOsInfo;
        }
    }
}
