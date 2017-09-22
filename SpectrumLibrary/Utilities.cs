using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpectrumLibrary
{
    static class Utilities
    {
        public static double? TryParseDouble(string text)
        {
            double parsedValue;
            if (double.TryParse(text, out parsedValue) && !double.IsNaN(parsedValue) && !double.IsInfinity(parsedValue))
            {
                return parsedValue;
            }
            else
                return null;
        }
    }
}
