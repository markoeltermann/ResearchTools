using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasProgramGenerator
{
    public class GeneratorArgs
    {

        public TimeSpan ProgramDuration { get; set; }

        public bool HasNitrogenCycles { get; set; }

        public double MaxOxygenConcentration { get; set; }

        public double MinOxygenConcentration { get; set; }

        public TimeSpan MinOxygenCycleDuration { get; set; }

        public TimeSpan MaxOxygenCycleDuraton { get; set; }

        public TimeSpan MinNitrogenCycleDuration { get; set; }

        public TimeSpan MaxNitrogenCycleDuration { get; set; }

    }
}
