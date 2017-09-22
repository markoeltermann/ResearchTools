using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Smoothing
{
    public class SquareSmoother
    {

        public int SquareWidth { get; private set; }

        public SquareSmoother(int squareWidth)
        {
            this.SquareWidth = squareWidth;
        }

        public double CalculateSmoothedValue<T>(T[] data, Func<T, double> valueFunction, int location)
        {
            Contract.Requires(data != null);
            Contract.Requires(valueFunction != null);
            Contract.Requires(location >= 0 && location < data.Length);

            double average = 0.0;
            int counter = 0;

            for (int i = location; i < Math.Min(data.Length, location + SquareWidth); i++)
            {
                double currentValue = valueFunction(data[i]);
                average += currentValue;
                counter++;
            }
            //average /= counter;


            return average;
        }

    }
}
