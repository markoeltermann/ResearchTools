using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    public enum SifCalibrationAxis
    {
        // Using large numbers to force size to an integer
        CalibX = 0x40000000,
        CalibY = 0x40000001,
        CalibZ = 0x40000002
    }
}
