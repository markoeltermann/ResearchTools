using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Utilites
{
    public static class Calculations
    {

        public static double RoundToOneDigit(this double value)
        {
            var ratio = Math.Pow(10, (int)Math.Floor(Math.Log10(value)));
            value = Math.Round(value / ratio) * ratio;
            return value;
        }

        public static double RoundToNDigits(this double value, int n)
        {
            if (n <= 0)
                throw new ArgumentException();

            var ratio = Math.Pow(10, (int)Math.Floor(Math.Log10(value)) - n + 1);
            value = Math.Round(value / ratio) * ratio;
            return value;
        }

    }
}
