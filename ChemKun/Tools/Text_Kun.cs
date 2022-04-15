using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;              //去掉字符串中空格用

namespace ChemKun.Tools
{
    class Text_Kun
    {
        /// <summary>
        /// 获取字符串中的所有词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] GetWords(string str)
        {
            string[] words;                                                           //字符串中的所有关键词
            str = str.Trim();
            words = Regex.Split(str, "\\s+", RegexOptions.IgnoreCase);                //忽略大小写
            return words;
        }
    }
}
