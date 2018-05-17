using SpectrumLibrary.GasScripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpectrumLibrary;
using System.Globalization;

namespace GasProgramGenerator
{
    class Program
    {
        private const string ParametersFilePath = @"C:\Data\Research\Raw data\2017-11-22\gas_program_parameters.txt";
        private static NumberFormatInfo Nfi = CultureInfo.InvariantCulture.NumberFormat;

        static void Main(string[] args)
        {

            //var programSections = new List<IProgramSection>()
            //{
            //    new ProgramSection(10, 100, 0.1),
            //    new ProgramSection(10, 0.1, 1),
            //    new ProgramSection(10, 1, 0.001),
            //    new ProgramStep(10, 0.001),
            //    new ProgramSection(5, 0.002, 0.1),
            //    new ProgramSection(5, 0.001, 100),

            //    new ProgramSection(10, 100, 0.1),
            //    new ProgramSection(10, 0.1, 1),
            //    new ProgramSection(10, 1, 0.001),
            //    new ProgramStep(15, 0.001),
            //    new ProgramSection(5, 0.002, 0.1),
            //    new ProgramSection(5, 0.001, 100),

            //    new ProgramStep(5, 100),
            //};

            //var programSections = new List<IProgramSection>()
            //{
            //    new ProgramStep(10, 5),
            //    new ProgramStep(10, 0),
            //    new ProgramStep(10, 5),
            //    new ProgramStep(10, 0),
            //    new ProgramStep(10, 10),
            //    new ProgramStep(10, 0),
            //    new ProgramStep(10, 20),
            //    new ProgramStep(10, 0),
            //    new ProgramStep(10, 40),
            //    new ProgramStep(10, 0),
            //    new ProgramStep(10, 100),
            //    new ProgramStep(10, 0),
            //};

            var randomProgramConfig = XYAsciiFileReader.ReadFileFirstColumn(ParametersFilePath);

            var programDuration = new TimeSpan(50, 0, 0);
            var generator = new SinusoidalGasProgramGenerator(randomProgramConfig.Select(xy => xy.X).ToArray(), randomProgramConfig.Select(xy => xy.Y).ToArray(), programDuration);

            var programSections = generator.GetProgramSteps();

            var programSteps = programSections.SelectMany(section => section.GetSteps()).ToList();

            var script = new StringBuilder();

            var timeFromStart = TimeSpan.Zero;
            for (int i = 0; i < programSteps.Count; i++)
            {
                var step = programSteps[i];
                var nextStep = i < programSteps.Count - 1 ? programSteps[i + 1] : null;
                //Console.Write(step.GetScriptLines(nextStep));
                script.Append(step.GetScriptLines(nextStep, timeFromStart));
                timeFromStart += step.Duration;
            }

            using (var sw = new StreamWriter(@"C:\Data\GeneratedGasProgramLabmeter.txt", false))
            {
                sw.Write(script.ToString());
            }
            using (var sw = new StreamWriter(@"C:\Data\GeneratedGasProgramMatcherInput.json", false))
            {
                sw.Write(JsonConvert.SerializeObject(programSteps, Formatting.Indented));
            }

            using (var sw = new StreamWriter(@"C:\Data\GeneratedGasProgramTest.txt", false))
            {
                var timeInHours = 0.0;
                foreach (var step in programSteps)
                {

                    sw.WriteLine($"{timeInHours.ToString(Nfi)}\t{step.OxygenConcentration.ToString(Nfi)}");

                    timeInHours += 1.0 / 180;
                }
            }

            var bronkhorstGasProgramJson = JsonConvert.SerializeObject(programSteps.Select(step => new JsonScriptLine()
            {
                Sp1 = step.OxygenConcentration,
                Sp2 = 0.0,
                Sp3 = 100.0 - step.OxygenConcentration,
                Time = step.Duration,
            }), Formatting.Indented);
            File.WriteAllText(@"C:\Data\GeneratedGasProgramBronkhorst.json", bronkhorstGasProgramJson);

            Console.WriteLine(script.ToString());
            Console.WriteLine($"Total duration = {programSteps.Sum(s => s.Duration.TotalHours)} hours.");



            Console.ReadLine();

        }
    }
}
