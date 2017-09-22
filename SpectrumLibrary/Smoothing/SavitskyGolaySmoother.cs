using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Smoothing
{
    public class SavitskyGolaySmoother
    {

        public int HalfWidth { get; private set; }

        public SavitskyGolaySmoother(int halfWidth)
        {
            this.HalfWidth = halfWidth;
        }

        public double CalculateSmoothedValue<T>(T[] data, Func<T, double> valueFunction, int location)
        {
            List<XYPoint> fittingInput = new List<XYPoint>();
            for (int i = Math.Max(0, location - HalfWidth); i < Math.Min(data.Length, location + HalfWidth); i++)
            {
                fittingInput.Add(new XYPoint(i, valueFunction(data[i])));
            }

            var polynomialCoefficients = Fitting.DoFittingReturnCoeffs(fittingInput, 2);
            return Fitting.GetPolynomialValueAt(location, polynomialCoefficients);
        }

    }
}
