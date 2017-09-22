using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Integration
{
    public class Integrator
    {

        public static double LinearIntegrate(List<double> xValues, List<double> yValues, double from, double to)
        {
            int i = 0;
            double result = 0;
            while (xValues[i] < from)
                i++;
            while (xValues[i] < to && i < xValues.Count - 2)
            {
                double v1 = yValues[i];
                double v2 = yValues[i + 1];
                double delta = xValues[i + 1] - xValues[i];
                if (v1 < v2)
                {
                    result += v1 * delta + (v2 - v1) * delta / 2.0;
                }
                else
                {
                    result += v2 * delta + (v1 - v2) * delta / 2.0;
                }
                i++;
            }
            return result;
        }

    }
}
