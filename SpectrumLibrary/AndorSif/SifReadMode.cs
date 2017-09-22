using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    public enum SifReadMode
    {
        ReadAll = 0x40000000,
        ReadHeaderOnly = 0x40000001
    }
}
