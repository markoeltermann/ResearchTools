using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    public enum SifResult : uint
    {
        None = 0,

        Success = 22002,

        SifFormatError = 22003,
        NoSifLoaded = 22004,
        FileNotFound = 22005,
        FileAccessError = 22006,
        DataNotPresent = 22007,

        P1Invalid = 22101,
        P2Invalid = 22102,
        P3Invalid = 22103,
        P4Invalid = 22104,
        P5Invalid = 22105,
        P6Invalid = 22106,
        P7Invalid = 22107,
        P8Invalid = 22108,

    }
}
