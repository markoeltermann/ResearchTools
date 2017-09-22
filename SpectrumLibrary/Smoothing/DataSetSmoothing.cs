using SpectrumLibrary.XYData;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Smoothing
{
    public class DataSetSmoothing
    {

        public static XYDataSet SmoothAndCompactDataSet(XYDataSet originalDataSet, int interval, int numberOfPointsToSkip)
        {
            Contract.Requires(originalDataSet != null);
            Contract.Requires(interval >= 1);

            XYDataSet smoothedDataSet = new XYDataSet(originalDataSet.SourceFileName, (originalDataSet.Points.Length - numberOfPointsToSkip) / interval);
            smoothedDataSet.Name = originalDataSet.Name;
            SmoothDataSet(originalDataSet, interval, smoothedDataSet, numberOfPointsToSkip);
            return smoothedDataSet;
        }

        public static XYDataSet<T> SmoothAndCompactDataSet<T>(XYDataSet<T> originalDataSet, int interval, int numberOfPointsToSkip)
        {
            Contract.Requires(originalDataSet != null);
            Contract.Requires(interval >= 1);

            XYDataSet<T> smoothedDataSet = new XYDataSet<T>(originalDataSet.SourceFileName, (originalDataSet.Points.Length - numberOfPointsToSkip) / interval, originalDataSet.MetaData);
            smoothedDataSet.Name = originalDataSet.Name;
            SmoothDataSet(originalDataSet, interval, smoothedDataSet, numberOfPointsToSkip);
            return smoothedDataSet;
        }

        private static void SmoothDataSet(XYDataSet originalDataSet, int interval, XYDataSet smoothedDataSet, int numberOfPointsToSkip)
        {
            //SavitskyGolaySmoother smoother = new SavitskyGolaySmoother(interval);
            double shift = originalDataSet.Points[numberOfPointsToSkip].X;
            SquareSmoother smoother = new SquareSmoother(interval);
            for (int i = 0; i < smoothedDataSet.Points.Length; i++)
            {
                smoothedDataSet.Points[i].X = originalDataSet.Points[i * interval + interval / 2 + numberOfPointsToSkip].X - shift;
                smoothedDataSet.Points[i].Y = smoother.CalculateSmoothedValue(originalDataSet.Points, x => x.Y, i * interval + numberOfPointsToSkip);
            }
        }

    }
}
