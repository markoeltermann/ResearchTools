using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.XYData
{
    public class XYAsciiFileReader<TXYDataSet>
    {

        private string _SourceFileFullPath;
        public string SourceFileFullPath
        {
            get { return _SourceFileFullPath; }
            set
            {
                Contract.Requires(value != null);
                _SourceFileFullPath = value;
            }
        }

        private char[] _Delimiters;
        public char[] Delimiters
        {
            get { return _Delimiters; }
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value.Length > 0);
                _Delimiters = value;
            }
        }

        private bool _UseInvariantCultureForParsing;

        public bool UseInvariantCultureForParsing
        {
            get { return _UseInvariantCultureForParsing; }
            set
            {
                _UseInvariantCultureForParsing = value;
                if (value)
                    numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                else
                    numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            }
        }

        private NumberFormatInfo numberFormat;

        public XYAsciiFileReader(string sourceFileFullPath)
        {
            this.SourceFileFullPath = sourceFileFullPath;
            this.Delimiters = new char[] { '\t' };
            this.UseInvariantCultureForParsing = true;
        }

        [Pure]
        public TXYDataSet[] ReadMultipleColumnFile(IXYDataSetFactory<TXYDataSet> dataSetFactory)
        {
            var dataSetNames = new List<string>();
            var result = new List<List<XYPoint>>();

            using (StreamReader sr = new StreamReader(SourceFileFullPath))
            {
                if (sr.EndOfStream)
                    return new TXYDataSet[0];

                string line = sr.ReadLine();
                ParseFirstLine(dataSetNames, result, line);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    ParseLine(result, line);
                }
            }
            TXYDataSet[] resultDataSets = new TXYDataSet[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                resultDataSets[i] = dataSetFactory.CreateDataSet(result[i].ToArray(), dataSetNames[i], SourceFileFullPath);
            }
            return resultDataSets;
        }

        private void ParseLine(List<List<XYPoint>> result, string line)
        {
            var pieces = line.Split(Delimiters);
            if (pieces.Length == result.Count + 1)
            {
                var parsedValues = TryParseAsciiFileNumericLine(pieces, numberFormat);
                if (parsedValues != null)
                {
                    for (int i = 1; i < parsedValues.Length; i++)
                    {
                        result[i - 1].Add(new XYPoint(parsedValues[0], parsedValues[i]));
                    }
                }
            }
        }

        private void ParseFirstLine(List<string> dataSetNames, List<List<XYPoint>> result, string line)
        {
            string[] pieces = line.Split(Delimiters);
            double[] parsedValues = TryParseAsciiFileNumericLine(pieces, numberFormat);
            if (parsedValues == null)
            {
                dataSetNames.AddRange(pieces.Skip(1));
                for (int i = 1; i < pieces.Length; i++)
                {
                    result.Add(new List<XYPoint>());
                }
            }
            else
            {
                for (int i = 1; i < parsedValues.Length - 1; i++)
                {
                    var list = new List<XYPoint>();
                    list.Add(new XYPoint(parsedValues[0], parsedValues[i]));
                    result.Add(list);
                }
            }
        }

        private static double[] TryParseAsciiFileNumericLine(string[] pieces, NumberFormatInfo nf)
        {
            double[] result = new double[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                if (!double.TryParse(pieces[i], NumberStyles.Float, nf, out result[i]))
                {
                    return null;
                }
            }
            return result;
        }


    }
}
