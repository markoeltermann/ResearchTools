using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary.Utilites;

namespace SpectrumLibraryTests.Utilities
{
    [TestFixture]
    public class CalculationsTests
    {
        [Test]
        public void RoundToOneDigitTest1()
        {
            double d;

            d = Calculations.RoundToOneDigit(18.4);
            Assert.That(d, Is.EqualTo(20.0));

            d = Calculations.RoundToOneDigit(13.4);
            Assert.That(d, Is.EqualTo(10.0));

            d = Calculations.RoundToOneDigit(1.45);
            Assert.That(d, Is.EqualTo(1.0));

            d = Calculations.RoundToOneDigit(0.237);
            Assert.That(d, Is.EqualTo(0.2));

        }

        [Test]
        public void RoundToNDigitsTest1()
        {
            double d;

            d = Calculations.RoundToNDigits(18.4, 2);
            Assert.That(d, Is.EqualTo(18.0));

            d = Calculations.RoundToNDigits(13.4, 3);
            Assert.That(d, Is.EqualTo(13.4));

            d = Calculations.RoundToNDigits(1.451, 2);
            Assert.That(d, Is.EqualTo(1.5));

            d = Calculations.RoundToNDigits(0.237234, 3);
            Assert.That(d, Is.EqualTo(0.237).Within(0.0000001));

        }

    }
}
