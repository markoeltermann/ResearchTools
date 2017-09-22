using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary.Utilites;

namespace SpectrumLibrary.GasScripting
{
    public class ProgramStep : IProgramSection
    {

        private const double MidSmallMfcCrossoverConcentration = 0.15;

        public ProgramStep()
        {

        }

        public ProgramStep(TimeSpan duration, double oxygenConcentration)
        {
            this.Duration = duration;
            this.OxygenConcentration = oxygenConcentration;
        }

        public ProgramStep(int durationInMinutes, double oxygenConcentration)
        {
            this.Duration = new TimeSpan(0, durationInMinutes, 0);
            this.OxygenConcentration = oxygenConcentration;
        }

        private static NumberFormatInfo Nfi = CultureInfo.InvariantCulture.NumberFormat;

        public TimeSpan Duration { get; set; }

        public double OxygenConcentration { get; set; }

        public string GetScriptLines(ProgramStep nextStep, TimeSpan? timeFromStart = null)
        {
            string s = Environment.NewLine + "//";
            if (timeFromStart.HasValue)
            {
                s += $" {timeFromStart.Value.TotalHours.ToString("0.00", Nfi)} h:";
            }

            s += $" Oxygen {OxygenConcentration.RoundToNDigits(3).ToString(Nfi)} %" + Environment.NewLine;
            

            double oxygenConcentrationIndicator = (Math.Log(this.OxygenConcentration) - Math.Log(0.001)) / (Math.Log(100) - Math.Log(0.001)) * 50.0;
            int oxygenConcentationIndicatorBarCount = (int)Math.Round(oxygenConcentrationIndicator);
            s += new string(Enumerable.Repeat('█', oxygenConcentationIndicatorBarCount).ToArray()) + Environment.NewLine;

            bool transitionToSmallMfc = nextStep != null && this.OxygenConcentration >= MidSmallMfcCrossoverConcentration && nextStep.OxygenConcentration < MidSmallMfcCrossoverConcentration;
            bool transitionToMidMfc = nextStep != null && this.OxygenConcentration < MidSmallMfcCrossoverConcentration && nextStep.OxygenConcentration >= MidSmallMfcCrossoverConcentration;
            var actualDuration = (transitionToSmallMfc || transitionToMidMfc) ? Duration - new TimeSpan(0, 0, 30) : Duration;

            if (OxygenConcentration >= 4)
            {
                var oxygenFlowRate = OxygenConcentration * 2;
                var nitrogenFlowRate = 200 - oxygenFlowRate;
                s += $"mfc::setflow 1=0, 2={nitrogenFlowRate.ToString("0.000", Nfi)}, 3={oxygenFlowRate.ToString("0.000", Nfi)}, 4=0, 5=0;";
                s += $"valve::pos 1=0, 2=1, 3=1, 4=0, 5=0;";
            }
            else if (OxygenConcentration >= MidSmallMfcCrossoverConcentration)
            {
                var oxygenFlowRate = OxygenConcentration * 20;
                var nitrogenFlowRate = 200 - oxygenFlowRate;
                s += $"mfc::setflow 1=0, 2={nitrogenFlowRate.ToString("0.000", Nfi)}, 3=0, 4={oxygenFlowRate.ToString("0.000", Nfi)}, 5=0;";
                s += $"valve::pos 1=0, 2=1, 3=0, 4=1, 5=0;";
            }
            else
            {
                var oxygenFlowRate = OxygenConcentration * 20;
                var nitrogenFlowRate = 200 - oxygenFlowRate;
                s += $"mfc::setflow 1=0, 2={nitrogenFlowRate.ToString("0.000", Nfi)}, 3=0, 4=0, 5={oxygenFlowRate.ToString("0.000", Nfi)};";
                s += $"valve::pos 1=0, 2=1, 3=0, 4=0, 5=1;";
            }

            if (actualDuration > TimeSpan.Zero)
                s += $"wait::time {actualDuration.Hours}:{actualDuration.Minutes}:{actualDuration.Seconds};";

            if (transitionToSmallMfc)
            {
                var nextOxygenFlowRate = nextStep.OxygenConcentration * 20;
                s += $"mfc::setflow 5={nextOxygenFlowRate.ToString("0.000", Nfi)};";
                s += $"wait::time 00:00:{Math.Min(30, Duration.Seconds)};";
            }
            if (transitionToMidMfc)
            {
                var nextOxygenFlowRate = nextStep.OxygenConcentration * 20;
                s += $"mfc::setflow 4={nextOxygenFlowRate.ToString("0.000", Nfi)};";
                s += $"wait::time 00:00:{Math.Min(30, Duration.Seconds)};";
            }

            s = s.Replace(";", ";" + Environment.NewLine);

            return s;
        }

        public IEnumerable<ProgramStep> GetSteps()
        {
            yield return this;
        }
    }
}
