using SpectrumLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace AsciiFileShortener
{
    class Program
    {

        private const string SourceFilePath = @"C:\Users\Marko\Desktop\kustumised.txt";
        private static NumberFormatInfo NumberFormatToUse = CultureInfo.InvariantCulture.NumberFormat;

        static void Main(string[] args)
        {
            int skipInterval = 100;
            string targetFilePath = SourceFilePath.Substring(0, SourceFilePath.LastIndexOf('.')) + "_short_" + skipInterval + ".txt";

            var points = XYAsciiFileReader.ReadFile(SourceFilePath, new char[] { '\t' }, false);

            using (StreamWriter sw = new StreamWriter(targetFilePath, false))
            {
                for (int i = 0; i < points[0].Count; i += skipInterval)
                {
                    string lineToWrite = points[0][i].X.ToString(NumberFormatToUse) + '\t';
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (j < points.Count - 1)
                        {
                            lineToWrite += points[j][i].Y.ToString(NumberFormatToUse) + '\t';
                        }
                        else
                        {
                            lineToWrite += points[j][i].Y.ToString(NumberFormatToUse);
                        }
                    }
                    sw.WriteLine(lineToWrite);
                }
            }

        }
    }
}
