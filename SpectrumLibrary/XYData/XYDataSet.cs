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
    public class XYDataSet
    {
        protected static NumberFormatInfo NumberFormat = CultureInfo.InvariantCulture.NumberFormat;


        public XYPoint[] Points { get; protected set; }
        public string SourceFileName { get; protected set; }
        public string Name { get; set; }

        public int PointCount
        {
            get { return Points.Length; }
        }

        public static XYDataSet ReadFromFile(string sourceFileFullPath, bool inferXColumn)
        {
            XYDataSet dataSet = new XYDataSet();
            dataSet.SourceFileName = Path.GetFileName(sourceFileFullPath);
            dataSet.Points = XYAsciiFileReader.ReadFileFirstColumnAsArray(sourceFileFullPath, inferXColumn, false);
            return dataSet;
        }

        public XYDataSet(string sourceFileName, int pointCount)
        {
            this.Points = new XYPoint[pointCount];
            this.SourceFileName = sourceFileName;
        }

        public XYDataSet(XYPoint[] points, string name)
        {
            if (points != null)
                this.Points = points;
            else
                this.Points = new XYPoint[0];
            this.Name = name;
        }

        protected XYDataSet()
        {

        }
    }

    public class XYDataSet<T> : XYDataSet
    {

        private XYDataSet()
        {

        }

        public XYDataSet(XYPoint[] points, string name)
            : base(points, name)
        { }

        public XYDataSet(string sourceFileName, int pointCount, T metaData)
            : base(sourceFileName, pointCount)
        {
            this.MetaData = metaData;
        }

        public T MetaData { get; set; }

        public static new XYDataSet<T> ReadFromFile(string sourceFileFullPath, bool inferXColumn)
        {
            XYDataSet<T> dataSet = new XYDataSet<T>();
            dataSet.SourceFileName = Path.GetFileName(sourceFileFullPath);
            dataSet.Points = XYAsciiFileReader.ReadFileFirstColumnAsArray(sourceFileFullPath, inferXColumn, false);
            return dataSet;
        }

    }
}
