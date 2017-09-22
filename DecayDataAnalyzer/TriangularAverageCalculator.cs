using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecayDataAnalyzer
{
    public class TriangularAverageCalculator
    {
        public double[] Coefficients { get; private set; }
        public int HalfWidth { get; private set; }

        public TriangularAverageCalculator(int halfWidth)
        {
            Contract.Requires(halfWidth > 0);

            this.HalfWidth = halfWidth;
            InitializeCoefficients();
        }

        private void InitializeCoefficients()
        {
            Coefficients = new double[HalfWidth * 2 + 1];
            for (int i = 0; i < HalfWidth; i++)
            {
                double coefficient = (double)i / HalfWidth;
                Coefficients[i] = coefficient;
                Coefficients[Coefficients.Length - i - 1] = coefficient;
            }
            Coefficients[HalfWidth] = 1.0;
        }

        public double CalculateAverage<T>(T[] data, Func<T, double> valueFunction, int location)
        {
            Contract.Requires(data != null);
            Contract.Requires(valueFunction != null);
            Contract.Requires(location >= 0 && location < data.Length);

            double average = 0.0;
            int counter = 0;

            if (location > 0)
            {
                for (int i = location - HalfWidth; i <= location + HalfWidth; i++)
                {
                    double currentValue = valueFunction(data[location]);
                    average += currentValue * Coefficients[counter++];
                }
                average /= HalfWidth;
            }
            else
            {
                for (int i = 0; i <= HalfWidth; i++)
                {
                    double currentValue = valueFunction(data[location]);
                    average += currentValue * Coefficients[counter++];
                }
                average /= (HalfWidth - 1) / 2.0 + 1;
            }

            return average;
        }

    }

}
