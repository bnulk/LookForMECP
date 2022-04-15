using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.Input
{
    class HandleCmdLine
    {
        private string[] args;                                //输入参量字符串
        private bool isHelp;                                  //是否请求帮助，“是”为帮助信息；“否”为标准输入，即："Usage: (program) input [output] [prefix]"
        private bool isFinish;                                //是否直接终结程序。

        private string helpRequest;                           //帮助请求，帮助请求的第一个字符必须是"-"



        public HandleCmdLine(string[] args, ref Data_Cmd.CmdData cmdData)
        {
            this.args = args;
            //什么都没有输入
            if (args.Length == 0)
            {
                Console.WriteLine("\n" + "No valid input found" + "\n");
                Console.WriteLine("\n" + "Usage: (program) input [output] [prefix]" + "\n");
                Console.WriteLine("Or: (program) \"-about\"" + "\n");
                return;
            }

            //有输入，判断是第一类输入，还是第二类输入。
            isHelp = JudgyIsHelp(args);                          //判断依据，第一个字符是否是"-"
            //第一类输入
            if (isHelp == true)
            {
                HelpInformation(args[0]);                       //处理第一类输入。即控制台输出信息，或者报错。
            }
            //第二类输入
            else
            {
                GetInputInformation(args, ref cmdData);         //处理第二类输入。根据输入参数，给Data_Input对象下的CmdData结构赋值。
            }
        }


        /// <summary>
        /// 根据第一个字符串的第一个字符是否"-"，判断是否请求帮助
        /// </summary>
        /// <param name="args">参数数组</param>
        /// <returns></returns>
        private bool JudgyIsHelp(string[] args)
        {
            if (args[0].Length > 1)
                return (args[0].Remove(1) == "-");
            else
                return false;
        }

        /// <summary>
        /// 输出帮助信息
        /// </summary>
        /// <param name="v">输入帮助请求的字符串</param>
        private void HelpInformation(string v)
        {
            switch (v)
            {
                case "-about":
                    Console.WriteLine("\n");
                    Console.WriteLine("ChemChemKun 1.0" + "\n");
                    Console.WriteLine("Author Information: Liu Kun, College of Chemistry, Tianjin Normal University, Tianjin 300387, China" + "\n");
                    Console.WriteLine("Email: bnulk@foxmail.com" + "\n");
                    Console.WriteLine("You can obtain the newest version of the program from the authors." + "\n");
                    break;
                default:
                    Console.WriteLine("input error." + "\n");
                    Console.WriteLine("right key word of request: \"-about\"" + "\n");
                    break;
            }
            return;
        }

        /// <summary>
        /// 根据输入信息，获得输入参数个数，输入文件名，输入文件路径，输出文件名和参数
        /// </summary>
        /// <param name="args">程序的输入字符串数组</param>
        private void GetInputInformation(string[] args, ref Data_Cmd.CmdData cmdData)
        {
            //参数个数
            cmdData.nWord = args.Length;
            //当前所在目录
            cmdData.directoryName = System.IO.Directory.GetCurrentDirectory();
            //临时没有扩展名的文件名
            string tmpNameWithoutExtension = "";

            //根据参数个数，给相应参数赋值。
            if (args.Length == 0)
            {
                Console.WriteLine("\n" + "No valid input found" + "\n");
                Console.WriteLine("\n" + "Usage: (program) input [output] [prefix]" + "\n");
                return;
            }
            else
            {
                if (args.Length == 1)
                {
                    tmpNameWithoutExtension = ObtainNameWithoutExtension(args[0], ".gjf");
                    cmdData.inputName = tmpNameWithoutExtension + ".gjf";
                    cmdData.outputName = tmpNameWithoutExtension + ".kun";
                    cmdData.param = null;
                }
                else
                {
                    if (args.Length == 2)
                    {
                        tmpNameWithoutExtension = ObtainNameWithoutExtension(args[0], ".gjf");
                        cmdData.inputName = tmpNameWithoutExtension + ".gjf";                      //分两步，是因为在ObtainNameWithoutExtension方法中有扩展名校验。
                        tmpNameWithoutExtension = ObtainNameWithoutExtension(args[1], ".kun");
                        cmdData.outputName = tmpNameWithoutExtension + ".kun";
                        cmdData.param = null;
                    }
                    else
                    {
                        if (args.Length == 3)
                        {
                            tmpNameWithoutExtension = ObtainNameWithoutExtension(args[0], ".gjf");
                            cmdData.inputName = tmpNameWithoutExtension + ".gjf";                      //分两步，是因为在ObtainNameWithoutExtension方法中有扩展名校验。
                            tmpNameWithoutExtension = ObtainNameWithoutExtension(args[1], ".kun");
                            cmdData.outputName = tmpNameWithoutExtension + ".kun";
                            cmdData.param = args[2];
                        }
                        else
                        {
                            Console.WriteLine("\n" + "No valid input found" + "\n");
                            Console.WriteLine("\n" + "Usage: (program) input [output] [prama]" + "\n");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从带扩展名的字符串中获取名字（包含目录信息）
        /// </summary>
        /// <param name="fileName">带扩展名的字符串</param>
        /// <param name="extension">扩展名</param>
        /// <returns></returns>
        private string ObtainNameWithoutExtension(string fileName, string extension)
        {
            string fileNameWithoutExtension = "";
            int extensionLength = extension.Length;
            try
            {
                if (fileName.Length > extensionLength)
                {
                    if (fileName.Remove(0, fileName.Length - extensionLength) == extension)                             //校验文件inputStr的扩展名是否为extension
                    {
                        fileNameWithoutExtension = fileName.Remove(fileName.Length - extensionLength);                  //校验成功，给返回值fileName赋予去掉扩展名的文件名
                    }
                    else
                    {
                        Console.WriteLine("Input.HandleCmdLine.ObtainName died." + "\n");                             //校验失败，提示文件的扩展名不正确
                        Console.WriteLine("The extension of file must be" + extension.ToString() + "\n");
                    }
                }
                else
                {
                    Console.WriteLine("The file name length is less than the set extension length" + "\n");
                }
            }
            catch
            {
                Console.WriteLine("Input.HandleCmdLine.ObtainName died." + "\n");
            }
            return fileNameWithoutExtension;
        }

    }
}
