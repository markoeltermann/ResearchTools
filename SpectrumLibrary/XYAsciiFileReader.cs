using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace SpectrumLibrary
{
    public class XYAsciiFileReader
    {
        private static NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;

        public static List<List<XYPoint>> ReadFile(string filePath)
        {
            return ReadFile(filePath, new char[] { '\t', ',' }, true);
        }


        public static List<List<XYPoint>> ReadFile(string filePath, char[] delimiters, bool useInvariantCultureForParsing)
        {
            List<List<XYPoint>> result = null; // = new List<XYPoint>();
            NumberFormatInfo nf = useInvariantCultureForParsing ? numberFormat : CultureInfo.CurrentCulture.NumberFormat;
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] pieces = line.Split(delimiters);
                    if (result == null)
                    {
                        result = new List<List<XYPoint>>();
                        for (int i = 0; i < pieces.Length - 1; i++)
                        {
                            result.Add(new List<XYPoint>());
                        }
                    }
                    if (pieces.Length == result.Count + 1)
                    {
                        double x;
                        double[] y = new double[pieces.Length];
                        bool fail;
                        if (double.TryParse(pieces[0], NumberStyles.Float, nf, out x))
                        {
                            fail = false;
                            for (int i = 0; i < result.Count; i++)
                            {
                                if (!double.TryParse(pieces[i + 1], NumberStyles.Float, nf, out y[i]))
                                {
                                    fail = true;
                                }
                            }
                            if (!fail)
                            {
                                for (int i = 0; i < result.Count; i++)
                                {
                                    result[i].Add(new XYPoint(x, y[i]));
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        //public static List<XYPoint> ReadFileFirstColumn(string filePath)
        //{
        //    List<XYPoint> result = new List<XYPoint>();          
        //    using (StreamReader sr = new StreamReader(filePath))
        //    {
        //        string line;
        //        while (!sr.EndOfStream)
        //        {
        //            line = sr.ReadLine();
        //            string[] pieces = line.Split('\t', ',');
        //            if (pieces.Length >= 2)
        //            {
        //                double x;
        //                double y;
        //                if (double.TryParse(pieces[0], NumberStyles.Float, numberFormat, out x))
        //                {
        //                    if (double.TryParse(pieces[1], NumberStyles.Float, numberFormat, out y))
        //                    {
        //                        result.Add(new XYPoint(x, y));
        //                    }                         
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public static List<XYPoint> ReadFileFirstColumn(string filePath)
        {
            List<XYPoint> result = new List<XYPoint>();
            string fileContents = File.ReadAllText(filePath);
            var lines = fileContents.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string[] pieces = line.Split('\t', ',');
                if (pieces.Length >= 2)
                {
                    if (double.TryParse(pieces[0], NumberStyles.Float, numberFormat, out var x))
                    {
                        if (double.TryParse(pieces[1], NumberStyles.Float, numberFormat, out var y))
                        {
                            result.Add(new XYPoint(x, y));
                        }
                    }
                }
            }
            //using (StreamReader sr = new StreamReader(filePath))
            //{
            //    string line;
            //    while (!sr.EndOfStream)
            //    {
            //        line = sr.ReadLine();
            //        string[] pieces = line.Split('\t', ',');
            //        if (pieces.Length >= 2)
            //        {
            //            double x;
            //            double y;
            //            if (double.TryParse(pieces[0], NumberStyles.Float, numberFormat, out x))
            //            {
            //                if (double.TryParse(pieces[1], NumberStyles.Float, numberFormat, out y))
            //                {
            //                    result.Add(new XYPoint(x, y));
            //                }
            //            }
            //        }
            //    }
            //}
            return result;
        }

        public static XYPoint[] ReadFileFirstColumnAsArray(string filePath)
        {
            return ReadFileFirstColumnAsArray(filePath, false, false);
        }

        public unsafe static XYPoint[] ReadFileFirstColumnAsArray(string filePath, bool inferXColumn, bool useCommaFix)
        {
            string fileContents = File.ReadAllText(filePath);
            return ReadFileContentsFirstColumnAsArray(inferXColumn, useCommaFix, fileContents);
        }

        public static unsafe XYPoint[] ReadFileContentsFirstColumnAsArray(bool inferXColumn, bool useCommaFix, string fileContents)
        {
            var lines = fileContents.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            XYPoint[] result = new XYPoint[lines.Length];

            int i = 0;
            int skippedLineCount = 0;

            fixed (XYPoint* resultp = result)
            {
                foreach (var line in lines)
                {
                    string[] pieces = line.Split('\t', ',');
                    if ((!useCommaFix && pieces.Length >= 2) || pieces.Length >= 3)
                    {
                        double x;
                        double y;
                        string xString;
                        string yString;
                        if (useCommaFix)
                        {
                            xString = $"{pieces[0]}.{pieces[1]}";
                            yString = pieces[2];
                        }
                        else
                        {
                            xString = pieces[0];
                            yString = pieces[1];
                        }
                        if (double.TryParse(xString, NumberStyles.Float, numberFormat, out x))
                        {
                            if (double.TryParse(yString, NumberStyles.Float, numberFormat, out y))
                            {
                                result[i].X = x;
                                result[i].Y = y;
                                i++;
                                continue;
                            }
                        }
                    }
                    else if (pieces.Length == 1 && inferXColumn)
                    {
                        double y;
                        if (double.TryParse(pieces[0], NumberStyles.Float, numberFormat, out y))
                        {
                            result[i].X = i;
                            result[i].Y = y;
                            i++;
                            continue;
                        }
                    }
                    skippedLineCount++;
                }
            }

            if (skippedLineCount != 0)
            {
                int itemCount = result.Length - skippedLineCount;
                XYPoint[] temp = new XYPoint[itemCount];
                //int xyPointSizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf(typeof(XYPoint));
                int xyPointSizeInBytes = sizeof(XYPoint);
                Buffer.BlockCopy(result, 0, temp, 0, xyPointSizeInBytes * itemCount);
                return temp;
            }
            else
                return result;
        }
    }
}
