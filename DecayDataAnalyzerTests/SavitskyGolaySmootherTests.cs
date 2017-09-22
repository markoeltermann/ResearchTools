using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecayDataAnalyzer;
using NUnit.Framework;
using SpectrumLibrary.Smoothing;


namespace DecayDataAnalyzerTests
{
    [TestFixture]
    public class SavitskyGolaySmootherTests
    {
        [Test]
        public void CalculateSmoothedValueTest()
        {

            double[] data = new double[100];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 10;
            }

            SavitskyGolaySmoother sgs = new SavitskyGolaySmoother(10);
            double smoothedValue = sgs.CalculateSmoothedValue(data, x => x, 50);
            Assert.AreEqual(10, smoothedValue, 0.000001);

        }

    }
}
