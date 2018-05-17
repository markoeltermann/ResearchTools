using Newtonsoft.Json;
using SpectrumLibrary;
using SpectrumLibrary.GasScripting;
using SpectrumLibrary.Interpolation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYDataMatcher.Model
{
    class Matcher
    {

        private static NumberFormatInfo Nfi = CultureInfo.InvariantCulture.NumberFormat;

        public string Source1FileName { get; set; }

        public string Source2FileName { get; set; }

        public string GasProgramFileName { get; set; }

        public double? RangeMaxX { get; set; }

        public double? RangeMinX { get; set; }

        public double ProgramStartTime { get; set; }

        public double GasChangeTimeConstant { get; set; }

        public double? CumulativeSignalDecayRate { get; set; }

        private List<double> outputXValues;
        private List<double> outputY1Values;
        private List<double> outputY2Values;
        private List<double> cumulativeOutputY1Values;
        private List<double> cumulativeOutputY2Values;
        private List<double> concentrations;

        public void Match()
        {
            var data1 = XYAsciiFileReader.ReadFileFirstColumn(Source1FileName);
            var data2 = XYAsciiFileReader.ReadFileFirstColumn(Source2FileName);

            List<ProgramStep> programSteps = null;
            if (!string.IsNullOrEmpty(GasProgramFileName))
            {
                programSteps = JsonConvert.DeserializeObject<List<ProgramStep>>(File.ReadAllText(GasProgramFileName));
            }

            if (RangeMaxX <= RangeMinX)
                return;

            var minX = Math.Min(data1.Min(p => p.X), data2.Min(p => p.X));
            var maxX = Math.Min(data1.Max(p => p.X), data2.Max(p => p.X));

            var originalRange = maxX - minX;

            if (RangeMinX.HasValue && minX < RangeMinX.Value)
                minX = RangeMinX.Value;
            if (RangeMaxX.HasValue && maxX > RangeMaxX.Value)
                maxX = RangeMaxX.Value;

            var newRange = maxX - minX;

            int pointCount = (data1.Count + data2.Count) / 2;

            pointCount = (int)(pointCount * newRange / originalRange);

            outputXValues = new List<double>();
            outputY1Values = new List<double>();
            outputY2Values = new List<double>();
            if (CumulativeSignalDecayRate.HasValue)
            {
                cumulativeOutputY1Values = new List<double>();
                cumulativeOutputY2Values = new List<double>();
            }

            var dx = maxX - minX;
            var step = dx / (pointCount - 1);
            var x = minX;

            for (int i = 0; i < pointCount; i++)
            {
                outputXValues.Add(x);
                outputY1Values.Add(data1.InterpolateAt(x));
                outputY2Values.Add(data2.InterpolateAt(x));

                if (CumulativeSignalDecayRate.HasValue)
                {
                    var cumulativeY1 = 0.0;
                    var cumulativeY2 = 0.0;
                    var totalWeight = 0.0;
                    for (int j = i; j >= 0; j--)
                    {
                        var cumX = outputXValues[j];
                        var weight = Math.Exp((cumX - x) / CumulativeSignalDecayRate.Value);
                        if (weight < 0.001)
                            break;
                        totalWeight += weight;
                        cumulativeY1 += outputY1Values[j] * weight;
                        cumulativeY2 += outputY2Values[j] * weight;
                    }
                    cumulativeY1 /= totalWeight;
                    cumulativeY2 /= totalWeight;

                    cumulativeOutputY1Values.Add(cumulativeY1);
                    cumulativeOutputY2Values.Add(cumulativeY2);
                }

                x += step;
            }

            if (programSteps != null && programSteps.Count != 0)
            {
                concentrations = new List<double>();
                programSteps.Add(new ProgramStep()
                {
                    Duration = TimeSpan.MaxValue,
                    OxygenConcentration = 0.0,
                });

                int currentStepIndex = 0;
                double currentStepStartTime = 0.0;
                var currentStep = programSteps[0];
                var previousStepConcentration = 0.0;

                foreach (var time in outputXValues)
                {
                    var relativeTime = time - ProgramStartTime;
                    double concentration = 0.0;

                    if (currentStepStartTime < relativeTime)
                    {
                        while (currentStepStartTime + currentStep.Duration.TotalSeconds < relativeTime)
                        {
                            currentStepIndex++;
                            currentStepStartTime += currentStep.Duration.TotalSeconds;
                            previousStepConcentration = currentStep.OxygenConcentration;
                            currentStep = programSteps[currentStepIndex];
                        }

                        var finalConcentration = currentStep.OxygenConcentration;
                        concentration = finalConcentration + (previousStepConcentration - finalConcentration) * Math.Exp(-(relativeTime - currentStepStartTime) / GasChangeTimeConstant);
                    }

                    concentrations.Add(concentration);
                }
            }

            WriteOutputToFile();

        }


        private void WriteOutputToFile()
        {
            var outputPath = Path.GetDirectoryName(Source1FileName) + "\\match.txt";

            using (var sw = new StreamWriter(outputPath))
            {
                WriteHeader(sw);

                for (int i = 0; i < outputXValues.Count; i++)
                {
                    sw.Write($"{outputXValues[i].ToString(Nfi)}\t{outputY1Values[i].ToString(Nfi)}\t{outputY2Values[i].ToString(Nfi)}");                   
                    if (cumulativeOutputY1Values != null)
                    {
                        sw.Write($"\t{cumulativeOutputY1Values[i].ToString(Nfi)}\t{cumulativeOutputY2Values[i].ToString(Nfi)}");
                    }
                    if (concentrations != null)
                    {
                        sw.Write($"\t{concentrations[i].ToString(Nfi)}");
                    }
                    sw.WriteLine();
                }
            }
        }

        private void WriteHeader(StreamWriter sw)
        {
            var y1Name = Path.GetFileNameWithoutExtension(Source1FileName);
            var y2Name = Path.GetFileNameWithoutExtension(this.Source2FileName);
            sw.Write($"x\t{y1Name}\t{y2Name}");
            if (cumulativeOutputY1Values != null)
            {
                sw.Write($"\t{y1Name}_cumulative\t{y2Name}_cumulative");
            }
            if (concentrations != null)
                sw.Write("\tconcentration");
            sw.WriteLine();
        }
    }
}
