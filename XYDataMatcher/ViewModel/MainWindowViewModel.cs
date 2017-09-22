using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SpectrumLibrary;
using SpectrumLibrary.Interpolation;
using System.IO;
using System.Globalization;
using SpectrumLibrary.GasScripting;
using Newtonsoft.Json;

namespace XYDataMatcher.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {

        private static NumberFormatInfo Nfi = CultureInfo.InvariantCulture.NumberFormat;

        public MainWindowViewModel()
        {
            MatchCommand = new RelayCommand(Match);
            _GasChangeTimeConstant = 3;
        }

        private void Match(object obj)
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

            var outputXValues = new List<double>();
            var outputY1Values = new List<double>();
            var outputY2Values = new List<double>();

            var dx = maxX - minX;
            var step = dx / (pointCount - 1);
            var x = minX;

            for (int i = 0; i < pointCount; i++)
            {
                outputXValues.Add(x);
                outputY1Values.Add(data1.InterpolateAt(x));
                outputY2Values.Add(data2.InterpolateAt(x));

                x += step;
            }

            List<double> concentrations = null;
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

            WriteOutputToFile(outputXValues, outputY1Values, outputY2Values, concentrations);

        }

        private void WriteOutputToFile(List<double> outputXValues, List<double> outputY1Values, List<double> outputY2Values, List<double> concentrations)
        {
            var outputPath = Path.GetDirectoryName(Source1FileName) + "\\match.txt";

            using (var sw = new StreamWriter(outputPath))
            {
                sw.WriteLine($"x\t{Path.GetFileNameWithoutExtension(Source1FileName)}\t{Path.GetFileNameWithoutExtension(Source2FileName)}\tconcentration");
                if (concentrations == null)
                {
                    for (int i = 0; i < outputXValues.Count; i++)
                    {
                        sw.WriteLine($"{outputXValues[i].ToString(Nfi)}\t{outputY1Values[i].ToString(Nfi)}\t{outputY2Values[i].ToString(Nfi)}");
                    }
                }
                else
                {
                    for (int i = 0; i < outputXValues.Count; i++)
                    {
                        sw.WriteLine($"{outputXValues[i].ToString(Nfi)}\t{outputY1Values[i].ToString(Nfi)}\t{outputY2Values[i].ToString(Nfi)}\t{concentrations[i].ToString(Nfi)}");
                    }
                }
            }
        }

        private string _Source1FileName;
        public string Source1FileName
        {
            get { return _Source1FileName; }
            set
            {
                _Source1FileName = value;
                OnPropertyChanged(nameof(Source1FileName));
            }
        }

        private string _Source2FileName;
        public string Source2FileName
        {
            get { return _Source2FileName; }
            set
            {
                _Source2FileName = value;
                OnPropertyChanged(nameof(Source2FileName));
            }
        }

        private string _GasProgramFileName;
        public string GasProgramFileName
        {
            get { return _GasProgramFileName; }
            set
            {
                _GasProgramFileName = value;
                OnPropertyChanged(nameof(GasProgramFileName));
            }
        }

        private double? _RangeMinX;
        public double? RangeMinX
        {
            get { return _RangeMinX; }
            set
            {
                _RangeMinX = value;
                OnPropertyChanged(nameof(RangeMinX));
            }
        }

        private double? _RangeMaxX;
        public double? RangeMaxX
        {
            get { return _RangeMaxX; }
            set
            {
                _RangeMaxX = value;
                OnPropertyChanged(nameof(RangeMaxX));
            }
        }

        private double _ProgramStartTime;
        public double ProgramStartTime
        {
            get { return _ProgramStartTime; }
            set
            {
                _ProgramStartTime = value;
                OnPropertyChanged(nameof(ProgramStartTime));
            }
        }

        private double _GasChangeTimeConstant;
        public double GasChangeTimeConstant
        {
            get { return _GasChangeTimeConstant; }
            set
            {
                _GasChangeTimeConstant = value;
                OnPropertyChanged(nameof(GasChangeTimeConstant));
            }
        }
        
        public ICommand MatchCommand { get; private set; }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


    }
}
