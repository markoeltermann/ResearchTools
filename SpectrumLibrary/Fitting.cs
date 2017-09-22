using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpectrumLibrary;
using SpectrumLibrary.Spectrum;

namespace SpectrumLibrary
{
    public class Fitting
    {

        public static double[] DoFittingReturnCoeffs(IList<XYPoint> points, SpectrumMetadata regions)
        {
            var filteredPoints = FilterPoints(points, regions);
            return DoFittingReturnCoeffs(filteredPoints, regions.FittingOrder);
        }

        public static double[] DoFittingReturnCoeffs(IList<XYPoint> points, int order)
        {
            var filteredPoints = points;
            DenseMatrix xMatrix = new DenseMatrix(filteredPoints.Count, order + 1);
            Vector yVector = new DenseVector(filteredPoints.Count);
            for (int i = 0; i < filteredPoints.Count; i++)
            {
                XYPoint point = filteredPoints[i];
                double[] rowData = new double[order + 1];
                rowData[0] = 1;
                for (int j = 1; j < rowData.Length; j++)
                {
                    rowData[j] = Math.Pow(point.X, j);
                }
                xMatrix.SetRow(i, rowData);
                yVector[i] = point.Y;
            }

            var xMatrixTransposed = xMatrix.Transpose();
            var result = (xMatrixTransposed * xMatrix).Inverse() * (xMatrixTransposed * yVector);
            double[] coeffs = result.ToArray();
            return coeffs;
        }

        private static IList<XYPoint> FilterPoints(IList<XYPoint> points, SpectrumMetadata regions)
        {
            List<XYPoint> result = new List<XYPoint>();
            foreach (var point in points)
            {
                if ((point.X > regions.FittingRegionA1 && point.X < regions.FittingRegionA2)
                    || (point.X > regions.FittingRegionB1 && point.X < regions.FittingRegionB2)
                    )
                {
                    result.Add(point);
                }
            }
            //for (int i = 0; i < 20; i++)
            //{
            //    result.Add(new XYPoint(1400 + i, 0));
            //}
            return result;
        }

        public static double GetPolynomialValueAt(double coordinate, double[] polynomialCoefficents)
        {
            double value = polynomialCoefficents[0];
            for (int j = 1; j < polynomialCoefficents.Length; j++)
            {
                value += Math.Pow(coordinate, j) * polynomialCoefficents[j];
            }
            return value;
        }

    }
}
