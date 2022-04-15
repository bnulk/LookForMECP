using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;

namespace ChemKun.Input
{
    partial class ReadInput
    {
        /// <summary>
        /// 把输入列表中的数据，按照高斯程序的输入格式，传递给Data_Input中的Data_Input_0_Gaussian。其中公共命令传递给Data_Input中的命令和参数部分。
        /// </summary>
        /// <param name="inputList">输入列表</param>
        /// <param name="data_Input">输入数据</param>
        private void DisposeGaussianInputList(List<string> inputList, ref Data_Input.GaussianInputSegment gaussianInputSegment)
        {
            string str = "";                                      //读取每行数据
            int iSegment = 0;                                     //分段的标识
            bool isChargeAndMultiplicity = true;                  //是否为电荷和自旋多重度的那一行
            gaussianInputSegment.coordinateType = null;               //坐标类型
            //初始化
            gaussianInputSegment.firstSection = new List<string>();
            gaussianInputSegment.routeSection = new List<string>();
            gaussianInputSegment.titleSection = new List<string>();
            gaussianInputSegment.chargeAndMultiplicity = "";
            gaussianInputSegment.addition = new List<string>();
            //填入数据
            for (int i = 0; i < inputList.Count; i++)
            {
                str = inputList[i];
                if (str.Trim() == "" && iSegment<=3)
                    iSegment++;
                else
                {
                    switch (iSegment)
                    {
                        case 0:
                            if (str.Substring(0, 1) == "%")
                            {
                                gaussianInputSegment.firstSection.Add(str);
                            }
                            else
                            {
                                gaussianInputSegment.routeSection.Add(str);
                            }
                            break;
                        case 1:
                            gaussianInputSegment.titleSection.Add(str);
                            break;
                        case 2:
                            if (isChargeAndMultiplicity == true)
                            {
                                gaussianInputSegment.chargeAndMultiplicity = str;
                                isChargeAndMultiplicity = false;
                                break;
                            }
                            else
                            {
                                if (gaussianInputSegment.coordinateType == null)                           //判断坐标类型
                                {
                                    if (str.Length < 4)                                                               //已经去掉前后的“ ”后，第一行的长度
                                    {
                                        gaussianInputSegment.coordinateType = "z-matrix";
                                        gaussianInputSegment.molecularSpecification_ZMatrix = new List<string>();
                                        gaussianInputSegment.molecularPara_ZMatrix = new List<string>();
                                    }
                                    else
                                    {
                                        gaussianInputSegment.coordinateType = "cartesian";
                                        gaussianInputSegment.molecularCartesian = new List<string>();
                                    }
                                }
                                if (gaussianInputSegment.coordinateType == "z-matrix")
                                    gaussianInputSegment.molecularSpecification_ZMatrix.Add(str);
                                else
                                    gaussianInputSegment.molecularCartesian.Add(str);
                            }
                            break;
                        case 3:
                            if (gaussianInputSegment.coordinateType == "z-matrix")
                                gaussianInputSegment.molecularPara_ZMatrix.Add(str);
                            break;
                        default:
                            gaussianInputSegment.addition.Add(str);
                            break;
                    }
                }
            }
            //获取原子个数N
            switch (gaussianInputSegment.coordinateType)
            {
                case "z-matrix":
                    gaussianInputSegment.N = gaussianInputSegment.molecularSpecification_ZMatrix.Count;
                    break;
                case "cartesian":
                    gaussianInputSegment.N = gaussianInputSegment.molecularCartesian.Count;
                    break;
                default:
                    break;
            }
            return;
        }


    }
}
