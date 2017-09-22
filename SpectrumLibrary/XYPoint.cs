using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SpectrumLibrary
{
    public struct XYPoint
    {
        private static readonly NumberFormatInfo InvariantNumberFormat = NumberFormatInfo.InvariantInfo;

        public XYPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;

        public double Y;

        public override string ToString()
        {
            return $"({X.ToString("0.00", InvariantNumberFormat)};{Y.ToString("0.00", InvariantNumberFormat)})";
        }

    }
}
