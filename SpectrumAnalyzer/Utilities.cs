using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace SpectrumAnalyzer
{
    static class Utilities
    {
        public static double? TryParseDouble(string text)
        {
            double parsedValue;
            if (double.TryParse(text, out parsedValue) && !double.IsNaN(parsedValue) && !double.IsInfinity(parsedValue))
            {
                return parsedValue;
            }
            else
                return null;
        }

        public static void RemoveByName(this SeriesCollection seriesCollection, string name)
        {
            Series s = seriesCollection.FindByName(name);
            if (s != null)
                seriesCollection.Remove(s);
        }

    }
}
