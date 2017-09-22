using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary;
using MathNet.Numerics.Interpolation;
using System.IO;
using System.Globalization;

namespace SpectrumCorrector
{
    /// <summary>
    /// Kalibratsioonilambi abil spektrite korrigeerija.
    /// </summary>
    class Program
    {

        //private const string DataFilePath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\2014_11_14\PLD550 IR parandatud lainepikkused.txt";
        //private const string CorrectionDataFilePath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\2014_11_14\IR intensiivsuse kalibratsioon.asc";

        private const string DataFilePath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\2014_11_14\PLD550 G-R.asc";
        private const string CorrectionDataFilePath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\2014_11_14\G-R intensiivsuse kalibratsioon.asc";
        
        private const string CalibrationDataFilePath = @"C:\Data\Kool\Bakatöö\Kalibratsioonilambi kalibratsiooniandmed\037990502_VIS_FIB.txt";

        private static readonly NumberFormatInfo InvariantNumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;


        static void Main(string[] args)
        {

            var data = XYAsciiFileReader.ReadFileFirstColumnAsArray(DataFilePath);
            var correctionData = XYAsciiFileReader.ReadFileFirstColumnAsArray(CorrectionDataFilePath);
            var calibarationData = XYAsciiFileReader.ReadFileFirstColumnAsArray(CalibrationDataFilePath);
            var calibrationXData = (from point in calibarationData
                                    select point.X).ToList();
            var calibrationYData = (from point in calibarationData
                                    select point.Y).ToList();
            var calibrationInterpolation = Interpolate.Common(calibrationXData, calibrationYData);

            var correctedData = new XYPoint[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                double x = data[i].X;
                double y = data[i].Y * calibrationInterpolation.Interpolate(x) / correctionData[i].Y;
                correctedData[i].X = x;
                correctedData[i].Y = y;
            }

            var outputPath = DataFilePath.Substring(0, DataFilePath.LastIndexOf('.')) + "_corrected.txt";

            using (StreamWriter sw = new StreamWriter(outputPath, false))
            {
                foreach (var point in correctedData)
                {
                    sw.WriteLine(point.X.ToString(InvariantNumberFormatInfo) + '\t' + point.Y.ToString(InvariantNumberFormatInfo));
                }
            }

        }

    }
}
