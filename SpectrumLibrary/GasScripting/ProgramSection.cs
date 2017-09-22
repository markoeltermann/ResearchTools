using SpectrumLibrary.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.GasScripting
{
    public class ProgramSection : IProgramSection
    {

        public ProgramSection()
        {

        }

        public ProgramSection(int timestepInMinutes, double initalOxygenConcentration, double finalOxygenConcentration)
        {
            this.StepDuration = new TimeSpan(0, timestepInMinutes, 0);
            this.InitialOxygenConcentration = initalOxygenConcentration;
            this.FinalOxygenConcentration = finalOxygenConcentration;
        }

        public TimeSpan StepDuration { get; set; }

        public double InitialOxygenConcentration { get; set; }

        public double FinalOxygenConcentration { get; set; }

        public IEnumerable<ProgramStep> GetSteps()
        {
            if (InitialOxygenConcentration == FinalOxygenConcentration)
                yield break;

            var oxygenConcentration = InitialOxygenConcentration;
            if (InitialOxygenConcentration > FinalOxygenConcentration)
            {
                while (true)
                {
                    oxygenConcentration = Calculations.RoundToOneDigit(oxygenConcentration);
                    if (oxygenConcentration <= FinalOxygenConcentration)
                        break;
                    yield return new ProgramStep()
                    {
                        Duration = StepDuration,
                        OxygenConcentration = oxygenConcentration,
                    };
                    oxygenConcentration /= 2.1;
                }
            }
            else
            {
                while (true)
                {
                    oxygenConcentration = Calculations.RoundToOneDigit(oxygenConcentration);
                    if (oxygenConcentration >= FinalOxygenConcentration)
                        break;
                    yield return new ProgramStep()
                    {
                        Duration = StepDuration,
                        OxygenConcentration = oxygenConcentration,
                    };
                    oxygenConcentration *= 2.3;
                }
            }
        }

    }
}
