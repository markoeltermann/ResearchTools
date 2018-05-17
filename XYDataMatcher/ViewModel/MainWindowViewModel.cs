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
using XYDataMatcher.Model;

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
            var matcher = new Matcher()
            {
                GasChangeTimeConstant = GasChangeTimeConstant,
                GasProgramFileName = GasProgramFileName,
                ProgramStartTime = ProgramStartTime,
                RangeMaxX = RangeMaxX,
                RangeMinX = RangeMinX,
                Source1FileName = Source1FileName,
                Source2FileName = Source2FileName,
                CumulativeSignalDecayRate = IsCumulativeSignalCalculationEnabled ? CumulativeSignalDecayRate : null,
            };
            matcher.Match();
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

        private bool _IsCumulativeSignalCalculationEnabled;
        public bool IsCumulativeSignalCalculationEnabled
        {
            get { return _IsCumulativeSignalCalculationEnabled; }
            set
            {
                _IsCumulativeSignalCalculationEnabled = value;
                OnPropertyChanged(nameof(IsCumulativeSignalCalculationEnabled));
            }
        }

        private double? _CumulativeSignalDecayRate;
        public double? CumulativeSignalDecayRate
        {
            get { return _CumulativeSignalDecayRate; }
            set
            {
                _CumulativeSignalDecayRate = value;
                OnPropertyChanged(nameof(CumulativeSignalDecayRate));
            }
        }


    }
}
