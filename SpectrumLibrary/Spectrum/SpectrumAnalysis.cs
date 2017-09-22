using SpectrumLibrary.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Spectrum
{
    public static class SpectrumAnalysis
    {

        public unsafe static SpectrumParameters AnalyzeSpectrum(XYPoint[] points, SpectrumMetadata spectrumRegions)
        {
            Contract.Requires(points != null);
            Contract.Requires(spectrumRegions != null);

            List<double> xValues = new List<double>(points.Length);
            List<double> yValues = new List<double>(points.Length);
            fixed (XYPoint* pPoints = points)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    xValues.Add(pPoints[i].X);
                    yValues.Add(pPoints[i].Y);
                }
            }

            double defectRadIntensity = 0.0;
            if (spectrumRegions.DefectRegionEnd != 0.0 && spectrumRegions.DefectRegionStart != 0.0)
            {
                defectRadIntensity = Integrator.LinearIntegrate(xValues, yValues, spectrumRegions.DefectRegionStart, spectrumRegions.DefectRegionEnd);
            }

            if (spectrumRegions.IsBaselineSubtractionEnabled)
            {
                double[] coeffs = Fitting.DoFittingReturnCoeffs(points, spectrumRegions);
                for (int i = 0; i < xValues.Count; i++)
                {
                    yValues[i] -= Fitting.GetPolynomialValueAt(xValues[i], coeffs);
                }
            }
            //lock (syncRoot)
            //{
            //    FileInfo fi = new FileInfo(path);
            //    using (StreamWriter sw = new StreamWriter(fi.Directory.FullName + "\\a_" + fi.Name, false))
            //    {
            //        for (int i = 0; i < xValues.Count; i++)
            //        {
            //            sw.WriteLine(xValues[i] + "\t" + yValues[i]);
            //        }
            //    }
            //}

            //IInterpolation interpolation = Interpolate.Common(xValues, yValues);
            //double reRadIntensity = Integrate.OnClosedInterval(x => interpolation.Interpolate(x), 565, 675);
            double reRadIntensity = Integrator.LinearIntegrate(xValues, yValues, spectrumRegions.ReRegionStart, spectrumRegions.ReRegionEnd);

            return new SpectrumParameters(defectRadIntensity, reRadIntensity);

        }

        


    }
}
