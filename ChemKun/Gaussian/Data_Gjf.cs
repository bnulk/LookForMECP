using System;
using System.Collections.Generic;
using System.Text;

namespace ChemKun.Gaussian
{
    class Data_Gjf
    {
        public struct GjfSegment
        {
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
        public GjfSegment gjfSegment;
    }
}
