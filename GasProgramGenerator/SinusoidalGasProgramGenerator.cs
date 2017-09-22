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

            while (timeFromStart < duration)
            {
                result.Add(new ProgramStep(new TimeSpan(0, 0, 20), GetOxygenConcentrationAt(timeFromStart)));

                timeFromStart += new TimeSpan(0, 0, 20);
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

            var minConcentration = 0.002;
            var maxConcentation = 0.8;

            concentration -= rawMin;
            concentration /= (rawMax - rawMin);

            concentration *= (Math.Log(maxConcentation) - Math.Log(minConcentration));
            concentration += (Math.Log(minConcentration));

            concentration = Math.Exp(concentration);

            return concentration;

        }
    }
}
