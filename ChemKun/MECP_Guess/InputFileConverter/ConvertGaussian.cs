using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemKun.Data;
using ChemKun.Gaussian;

namespace ChemKun.MECP_Guess.InputFileConverter
{
    class ConvertGaussian
    {
        public static void ToGjfSegment(Data_Input.GaussianInputSegment gaussianInputSegment, ref Data_Gjf.GjfSegment gjf1Segment,
            ref Data_Gjf.GjfSegment gjf2Segment, ref Data_Gjf.GjfSegment gjf3Segment, ref Data_Gjf.GjfSegment gjf4Segment)
        {
            int indexMark;
            int indexMark1;
            string str;
            string[] tmpStr = new string[2];
            for (int i = 0; i < gaussianInputSegment.firstSection.Count; i++)
            {
                if (gaussianInputSegment.firstSection[i].Substring(0, 4) == "%chk")
                {
                    tmpStr = gaussianInputSegment.firstSection[i].Split('=');
                    str = tmpStr[1].Trim();
                    tmpStr[0] = null; tmpStr[1] = null;
                    str.Replace(',', ' ');                                             //两个chk名字的分割                           //"LiuKun1.chk LiuKun2.chk"
                    indexMark = str.IndexOf(' ');
                    gjf1Segment.firstSection.Add("%chk=" + str.Substring(0, indexMark).Trim());                                      //"LiuKun1.chk"
                    gjf2Segment.firstSection.Add("%chk=" + str.Substring(indexMark, str.Length - indexMark).Trim());                   //" LiuKun2.chk"
                    gjf3Segment.firstSection.Add("%chk=" + str.Substring(0, indexMark).Trim());                                      //"LiuKun1.chk"
                    gjf4Segment.firstSection.Add("%chk=" + str.Substring(indexMark, str.Length - indexMark).Trim());                   //" LiuKun2.chk"
                }
                else
                {
                    gjf1Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                    gjf2Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                    gjf3Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                    gjf4Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                }
            }
            //routeSection部分
            str = "";
            for (int i = 0; i < gaussianInputSegment.routeSection.Count; i++)
            {
                str += gaussianInputSegment.routeSection[i];
            }
            indexMark = str.IndexOf('{');
            indexMark1 = str.IndexOf('}');
            str = str.Remove(indexMark, indexMark1 - indexMark + 1);
            gjf1Segment.routeSection.Add(str.Trim());
            gjf2Segment.routeSection.Add(str.Trim());
            gjf3Segment.routeSection.Add(str.Trim());
            gjf4Segment.routeSection.Add(str.Trim());
            //Title
            for (int i = 0; i < gaussianInputSegment.titleSection.Count; i++)
            {
                gjf1Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
                gjf2Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
                gjf3Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
                gjf4Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
            }
            //电荷和自旋多重度
            str = gaussianInputSegment.chargeAndMultiplicity.Trim();
            str = str.Replace(',', ' ');                                                                             //","和 " "等价
            indexMark = str.IndexOf(' ');
            gjf1Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            gjf2Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            gjf3Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            gjf4Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            str = gaussianInputSegment.chargeAndMultiplicity.Remove(0, indexMark).Trim();
            indexMark = str.IndexOf(' ');
            gjf1Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            gjf2Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            str = str.Remove(0, indexMark).Trim();
            indexMark = str.IndexOf(' ');
            if (indexMark == -1)
            {
                gjf3Segment.chargeAndMultiplicity += " " + str.Trim();
                gjf4Segment.chargeAndMultiplicity += " " + str.Trim();
            }
            else
            {
                gjf3Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
                gjf4Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            }
            //
            if (gaussianInputSegment.coordinateType.ToLower() == "z-matrix")
            {
                gjf1Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf2Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf3Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf4Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;

                List<string> molecularPara_ZMatrix_1 = new List<string>();
                List<string> molecularPara_ZMatrix_2 = new List<string>();
                SplitMolecularPara_ZMatrix(gaussianInputSegment.molecularPara_ZMatrix, ref molecularPara_ZMatrix_1, ref molecularPara_ZMatrix_2);

                gjf1Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_1;
                gjf2Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_2;
                gjf3Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_1;
                gjf4Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_2;
            }
            if (gaussianInputSegment.coordinateType.ToLower() == "cartesian")
            {
                Output.WriteOutput.Error.Append("ChemKun.MECP_Guess.InputFileConverter.ConvertGaussian died. Error. Cartesian coordinates are not supported by MECP guess algorithm :: Site ConvertGaussian" + "/n");
                Console.WriteLine("ChemKun.MECP_Guess.InputFileConverter.ConvertGaussian died. Error. Cartesian coordinates are not supported by MECP guess algorithm :: Site ConvertGaussian" + "/n");
                /*
                gjf1Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
                gjf2Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
                */
            }
            //附加部分
            gjf1Segment.addition = gaussianInputSegment.addition;
            gjf2Segment.addition = gaussianInputSegment.addition;
            gjf3Segment.addition = gaussianInputSegment.addition;
            gjf4Segment.addition = gaussianInputSegment.addition;
            return;
        }

        public static void ToGjfSegment(Data_Input.GaussianInputSegment gaussianInputSegment, ref Data_Gjf.GjfSegment gjf1Segment, ref Data_Gjf.GjfSegment gjf2Segment)
        {
            int indexMark;
            int indexMark1;
            string str;
            string[] tmpStr = new string[2];
            for (int i = 0; i < gaussianInputSegment.firstSection.Count; i++)
            {
                if (gaussianInputSegment.firstSection[i].Substring(0, 4) == "%chk")
                {
                    tmpStr = gaussianInputSegment.firstSection[i].Split('=');
                    str = tmpStr[1].Trim();
                    tmpStr[0] = null; tmpStr[1] = null;
                    str.Replace(',', ' ');                                             //两个chk名字的分割                           //"LiuKun1.chk LiuKun2.chk"
                    indexMark = str.IndexOf(' ');
                    gjf1Segment.firstSection.Add("%chk=" + str.Substring(0, indexMark).Trim());                                      //"LiuKun1.chk"
                    gjf2Segment.firstSection.Add("%chk=" + str.Substring(0, indexMark).Trim());                                      //"LiuKun1.chk"
                }
                else
                {
                    gjf1Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                    gjf2Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                }
            }
            //routeSection部分
            str = "";
            for (int i = 0; i < gaussianInputSegment.routeSection.Count; i++)
            {
                str += gaussianInputSegment.routeSection[i];
            }
            indexMark = str.IndexOf('{');
            indexMark1 = str.IndexOf('}');
            str = str.Remove(indexMark, indexMark1 - indexMark + 1);
            gjf1Segment.routeSection.Add(str.Trim());
            gjf2Segment.routeSection.Add(str.Trim());
            //Title
            for (int i = 0; i < gaussianInputSegment.titleSection.Count; i++)
            {
                gjf1Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
                gjf2Segment.titleSection.Add(gaussianInputSegment.titleSection[i].Trim());
            }
            //电荷和自旋多重度
            str = gaussianInputSegment.chargeAndMultiplicity.Trim();
            str = str.Replace(',', ' ');                                                                             //","和 " "等价
            indexMark = str.IndexOf(' ');
            gjf1Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            gjf2Segment.chargeAndMultiplicity = str.Substring(0, indexMark).Trim();
            str = gaussianInputSegment.chargeAndMultiplicity.Remove(0, indexMark).Trim();
            indexMark = str.IndexOf(' ');
            gjf1Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            str = str.Remove(0, indexMark).Trim();
            indexMark = str.IndexOf(' ');
            if (indexMark == -1)
            {
                gjf2Segment.chargeAndMultiplicity += " " + str.Trim();
            }
            else
            {
                gjf2Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            }
            //
            if (gaussianInputSegment.coordinateType.ToLower() == "z-matrix")
            {
                gjf1Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf2Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;

                List<string> molecularPara_ZMatrix_1 = new List<string>();
                List<string> molecularPara_ZMatrix_2 = new List<string>();
                SplitMolecularPara_ZMatrix(gaussianInputSegment.molecularPara_ZMatrix, ref molecularPara_ZMatrix_1, ref molecularPara_ZMatrix_2);

                gjf1Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_1;
                gjf2Segment.molecularPara_ZMatrix = molecularPara_ZMatrix_2;

            }
            if (gaussianInputSegment.coordinateType.ToLower() == "cartesian")
            {
                Output.WriteOutput.Error.Append("ChemKun.MECP_Guess.InputFileConverter.ConvertGaussian died. Error. Cartesian coordinates are not supported by MECP guess algorithm :: Site ConvertGaussian" + "/n");
                Console.WriteLine("ChemKun.MECP_Guess.InputFileConverter.ConvertGaussian died. Error. Cartesian coordinates are not supported by MECP guess algorithm :: Site ConvertGaussian" + "/n");
                /*
                gjf1Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
                gjf2Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
                */
            }
            //附加部分
            gjf1Segment.addition = gaussianInputSegment.addition;
            gjf2Segment.addition = gaussianInputSegment.addition;
            return;
        }

        /// <summary>
        /// 把zMatrix分子参量部分，按照标识“#####”，分成前后两部分。
        /// </summary>
        /// <param name="molecularPara_ZMatrix"></param>
        /// <param name="molecularPara_ZMatrix_1"></param>
        /// <param name="molecularPara_ZMatrix_2"></param>
        public static void SplitMolecularPara_ZMatrix(List<string> molecularPara_ZMatrix, ref List<string> molecularPara_ZMatrix_1, ref List<string> molecularPara_ZMatrix_2)
        {
            bool isFirstSection = true;
            for (int i = 0; i<molecularPara_ZMatrix.Count; i++)
            {
                if(molecularPara_ZMatrix[i].Trim()== "#####")
                {
                    isFirstSection = false;
                    i++;
                }

                if(isFirstSection==true)
                {
                    molecularPara_ZMatrix_1.Add(molecularPara_ZMatrix[i]);
                }
                else
                {
                    molecularPara_ZMatrix_2.Add(molecularPara_ZMatrix[i]);
                }
            }
        }


    }
}
