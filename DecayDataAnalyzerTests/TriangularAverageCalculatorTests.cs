using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DecayDataAnalyzer;

namespace DecayDataAnalyzerTests
{
    [TestFixture()]
    class TriangularAverageCalculatorTests
    {
        [Test()]
        public void CalculateAverageTest()
        {

            double[] data = new double[100];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 10;
            }

            TriangularAverageCalculator tac;

            tac = new TriangularAverageCalculator(10);
            var average = tac.CalculateAverage(data, x => x, 50);
            Assert.AreEqual(10, average, 0.000001);
            average = tac.CalculateAverage(data, x => x, 0);
            Assert.AreEqual(10, average, 0.000001);

            tac = new TriangularAverageCalculator(23);
            average = tac.CalculateAverage(data, x => x, 50);
            Assert.AreEqual(10, average, 0.000001);
            average = tac.CalculateAverage(data, x => x, 0);
            Assert.AreEqual(10, average, 0.000001);
        }

    }
}
