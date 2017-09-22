using System.Collections.Generic;

namespace SpectrumLibrary.GasScripting
{
    public interface IProgramSection
    {
        IEnumerable<ProgramStep> GetSteps();
    }
}