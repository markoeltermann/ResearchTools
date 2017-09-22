using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    public enum SifDataSource
    {
        // Using large numbers to force size to an integer
        Signal = 0x40000000,
        Reference = 0x40000001,
        Background = 0x40000002,
        Live = 0x40000003,
        Source = 0x40000004
    }
}
