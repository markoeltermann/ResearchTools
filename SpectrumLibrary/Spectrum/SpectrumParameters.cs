using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpectrumLibrary.Spectrum
{
    public class SpectrumParameters
    {

        internal SpectrumParameters(double defectRadiationIntensity, double reRadiationIntensity)
        {
            this.DefectRadiationIntensity = defectRadiationIntensity;
            this.ReRadiationIntensity = reRadiationIntensity;
        }

        public double DefectRadiationIntensity;

        public double ReRadiationIntensity;
    }
}
