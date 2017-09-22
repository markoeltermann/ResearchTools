using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary;
using SpectrumLibrary.Integration;
using System.IO;
using System.Globalization;

namespace DecayDataIntegrator
{
    class Program
    {

        public const string SourceFilePath = @"C:\Data\Research\Raw data\2017_07_13\SimplifiedData_10_3129.txt";

        static void Main(string[] args)
        {

            var data = XYAsciiFileReader.ReadFile(SourceFilePath);

            var integrationBounds = new[] { 200.0, 500.0, 1000.0, 2000.0, 5000.0 };
            var allIntegrations = new List<List<double>>();
            for (int i = 0; i < data.Count; i++)
            {
                allIntegrations.Add(new List<double>());
            }

            var xValues = data.First().Select(xy => xy.X).ToList();

            for (int columnNumber = 0; columnNumber < data.Count; columnNumber++)
            {
                var columnData = data[columnNumber];
                var integrations = allIntegrations[columnNumber];
                var yValues = columnData.Select(xy => xy.Y).ToList();

                foreach (var bound in integrationBounds)
                {
                    //var integral = Integrator.LinearIntegrate(xValues, yValues, 0, bound);
                    double integral = 0.0;
                    double averageTau = 0.0;
                    for (int i = 1; i < xValues.Count; i++)
                    {
                        if (xValues[i] <= bound)
                        {
                            integral += yValues[i];
                            averageTau += yValues[i] * xValues[i];
                        }
                        else
                            break;
                    }
                    averageTau /= integral;
                    integrations.Add(integral);
                    integrations.Add(averageTau);
                }

            }

            var outputFilePath = Path.GetDirectoryName(SourceFilePath) + "\\" + Path.GetFileNameWithoutExtension(SourceFilePath) + "_integrations.txt";
            using (var sw = new StreamWriter(outputFilePath, false))
            {
                sw.Write("dataset nr\t");
                sw.WriteLine(string.Join("\t", integrationBounds.SelectMany(b => new[] { b.ToString(), b.ToString() + " tau" })));
                for (int i = 0; i < allIntegrations.Count; i++)
                {
                    sw.Write(i + "\t");
                    var integrationList = allIntegrations[i];
                    sw.WriteLine(string.Join("\t", integrationList.Select(d => d.ToString(NumberFormatInfo.InvariantInfo))));
                }
            }

        }
    }
}
