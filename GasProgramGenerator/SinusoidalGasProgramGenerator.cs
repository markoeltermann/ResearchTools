using SpectrumLibrary.GasScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasProgramGenerator
{
    public class SinusoidalGasProgramGenerator
    {

        private double[] frequencies;

        private double[] phases;

        private TimeSpan duration;

        public SinusoidalGasProgramGenerator(double[] frequencies, double[] phases, TimeSpan duration)
        {
            this.frequencies = frequencies;
            this.phases = phases;
            this.duration = duration;
        }

        public List<ProgramStep> GetProgramSteps()
        {
            var result = new List<ProgramStep>();

            var timeFromStart = TimeSpan.Zero;
            var timeFromLastFlush = TimeSpan.Zero;

            while (timeFromStart < duration)
            {
                var stepDuration = new TimeSpan(0, 0, 20);
                var step = new ProgramStep(stepDuration, GetOxygenConcentrationAt(timeFromStart));
                result.Add(step);

                timeFromStart += stepDuration;
                timeFromLastFlush += stepDuration;
                if (timeFromLastFlush >= new TimeSpan(0, 30, 0))
                {
                    step.IncludesFlushLog = true;
                    timeFromLastFlush = TimeSpan.Zero;
                }
            }

            return result;
        }

        private double GetOxygenConcentrationAt(TimeSpan timeFromStart)
        {
            var concentration = 0.0;
            for (int i = 0; i < frequencies.Length; i++)
            {
                var frequency = frequencies[i];
                var phase = phases[i];

                concentration += Math.Pow(1 / frequency, 1.5)  * Math.Sin(timeFromStart.TotalHours * (frequency) + phase);
            }

            var rawMin = -2.5;
            var rawMax = 2.5;

            //var minConcentration = 0.002;
            //var maxConcentation = 0.8;

            var minConcentration = 0.1;
            var maxConcentation = 20;

            concentration -= rawMin;
            concentration /= (rawMax - rawMin);

            concentration *= (Math.Log(maxConcentation) - Math.Log(minConcentration));
            concentration += (Math.Log(minConcentration));

            concentration = Math.Exp(concentration);

            return concentration;

        }
    }
}
