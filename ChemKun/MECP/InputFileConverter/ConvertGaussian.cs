using System;
using System.Collections.Generic;
using System.Text;
using ChemKun.Data;
using ChemKun.Gaussian;

namespace ChemKun.MECP.InputFileConverter
{
    class ConvertGaussian
    {
        public static void ToGjfSegment(Data_Input.GaussianInputSegment gaussianInputSegment, ref Data_Gjf.GjfSegment gjf1Segment, ref Data_Gjf.GjfSegment gjf2Segment)
        {
            int indexMark;
            int indexMark1;
            string str;
            string[] tmpStr = new string[2];
            for(int i=0;i<gaussianInputSegment.firstSection.Count;i++)
            {
                if(gaussianInputSegment.firstSection[i].Substring(0,4)=="%chk")
                {
                    tmpStr = gaussianInputSegment.firstSection[i].Split('=');
                    str = tmpStr[1].Trim();
                    tmpStr[0] = null; tmpStr[1] = null;
                    str.Replace(',', ' ');                                             //两个chk名字的分割                           //"LiuKun1.chk LiuKun2.chk"
                    indexMark = str.IndexOf(' ');
                    gjf1Segment.firstSection.Add("%chk=" + str.Substring(0, indexMark).Trim());                                      //"LiuKun1.chk"
                    gjf2Segment.firstSection.Add("%chk=" + str.Substring(indexMark, str.Length-indexMark).Trim());                   //" LiuKun2.chk"
                }
                else
                {
                    gjf1Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                    gjf2Segment.firstSection.Add(gaussianInputSegment.firstSection[i].Trim());
                }
            }
            //routeSection部分
            str = "";
            for(int i=0;i<gaussianInputSegment.routeSection.Count;i++)
            {
                str += gaussianInputSegment.routeSection[i];
            }
            indexMark = str.IndexOf('{');
            indexMark1 = str.IndexOf('}');
            str = str.Remove(indexMark, indexMark1 - indexMark + 1);
            gjf1Segment.routeSection.Add(str.Trim());
            gjf2Segment.routeSection.Add(str.Trim());
            for (int i=0;i<gaussianInputSegment.titleSection.Count;i++)
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
            if(indexMark==-1)
            {
                gjf2Segment.chargeAndMultiplicity += " " + str.Trim();
            }
            else
            {
                gjf2Segment.chargeAndMultiplicity += " " + str.Substring(0, indexMark).Trim();
            }
            //
            if(gaussianInputSegment.coordinateType.ToLower()=="z-matrix")
            {
                gjf1Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf2Segment.molecularSpecification_ZMatrix = gaussianInputSegment.molecularSpecification_ZMatrix;
                gjf1Segment.molecularPara_ZMatrix = gaussianInputSegment.molecularPara_ZMatrix;
                gjf2Segment.molecularPara_ZMatrix = gaussianInputSegment.molecularPara_ZMatrix;
            }
            if(gaussianInputSegment.coordinateType.ToLower()=="cartesian")
            {
                gjf1Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
                gjf2Segment.molecularCartesian = gaussianInputSegment.molecularCartesian;
            }
            //附加部分
            gjf1Segment.addition = gaussianInputSegment.addition;
            gjf2Segment.addition = gaussianInputSegment.addition;
            return;
        }
    }
}
