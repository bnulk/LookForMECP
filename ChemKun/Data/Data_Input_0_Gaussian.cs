using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Data
{
    partial class Data_Input
    {
        public struct GaussianInputSegment
        {
            public int N;
            public List<string> firstSection;
            public List<string> routeSection;
            public List<string> titleSection;
            public string chargeAndMultiplicity;
            public string coordinateType;
            public List<string> molecularSpecification_ZMatrix;
            public List<string> molecularPara_ZMatrix;
            public List<string> molecularCartesian;
            public List<string> addition;
        }
        public GaussianInputSegment gaussianInputSegment;
    }
}
