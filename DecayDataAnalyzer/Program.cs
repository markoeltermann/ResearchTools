using SpectrumLibrary;
using SpectrumLibrary.Smoothing;
using SpectrumLibrary.XYData;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DecayDataAnalyzer
{
    static class Program
    {

        private static readonly NumberFormatInfo NumberFormatToUse = CultureInfo.InvariantCulture.NumberFormat;

        private static string[] fileFullPaths;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            if (!ShowDialogGetFilePathsToProcess())
            {
                return;
            }

            if (fileFullPaths.Length < 1)
            {
                return;
            }
            string dataDirectory = Path.GetDirectoryName(fileFullPaths[0]);

            int? compactionRatio = ShowDialogGetCompactionRatio(out var numberOfPointsToSkip);
            if (compactionRatio == null)
            {
                return;
            }
            else if (compactionRatio.Value < 1)
            {
                ShowErrorMessage("Vigane summeerimiskordaja");
                return;
            }
            DataParameters dataParameters;
            string memoFilePath = dataDirectory + "\\memo.txt";
            if (File.Exists(memoFilePath))
            {
                dataParameters = DataParameters.ReadFromFile(memoFilePath);
            }
            else
            {
                dataParameters = new DataParameters();
                dataParameters.InferXColumn = false;
            }

            List<XYDataSet> dataSets = new List<XYDataSet>(fileFullPaths.Length);
            foreach (var file in fileFullPaths)
            {
                dataSets.Add(XYDataSet.ReadFromFile(file, dataParameters.InferXColumn));
            }

            var pointCounts = from dataSet in dataSets
                              select dataSet.Points.Length;
            if (pointCounts.Distinct().Count() != 1)
            {
                ShowErrorMessage("Kõik sisendfailid pole võrdse punktide arvuga.");
                return;
            }

            //TriangularAverageCalculator tac = new TriangularAverageCalculator(100);
            //SavitskyGolaySmoother sgs = new SavitskyGolaySmoother(100);
            List<XYDataSet> simplifiedDataSets = new List<XYDataSet>(dataSets.Count);

            foreach (var dataSet in dataSets)
            {
                //XYDataSet simplifiedDataSet = new XYDataSet(dataSet.SourceFileName, dataSet.Points.Length / 100);
                //for (int i = 0; i < simplifiedDataSet.Points.Length; i++)
                //{
                //    //simplifiedDataSet.Points[i] = new XYPoint(i, tac.CalculateAverage(dataSet.Points, x => x.Y, i * 100));
                //    simplifiedDataSet.Points[i] = new XYPoint(i, sgs.CalculateSmoothedValue(dataSet.Points, x => x.Y, i * 100));
                //}
                var processedDataSet = DataSetSmoothing.SmoothAndCompactDataSet(dataSet, compactionRatio.Value, numberOfPointsToSkip.GetValueOrDefault(0));
                for (int i = 0; i < processedDataSet.PointCount; i++)
                {
                    processedDataSet.Points[i].X /= 1000.0;
                }
                simplifiedDataSets.Add(processedDataSet);
            }

            using (StreamWriter sw = new StreamWriter(dataDirectory + "\\SimplifiedData_" + compactionRatio + "_" + numberOfPointsToSkip + ".txt", false))
            {
                if (!dataParameters.UseXYZ)
                {
                    WriteXYData(dataParameters, simplifiedDataSets, sw);
                }
                else
                {
                    WriteXYZData(dataParameters, simplifiedDataSets, sw);
                }
            }
        }

        private static void WriteXYZData(DataParameters dataParameters, List<XYDataSet> simplifiedDataSets, StreamWriter sw)
        {
            string lineToWrite = "X\tY\tZ\t";
            sw.WriteLine(lineToWrite);
            foreach (var dataSet in simplifiedDataSets)
            {
                var parts = dataSet.SourceFileName.Split(' ', '.');
                var y = parts[1];
                foreach (var point in dataSet.Points)
                {
                    sw.WriteLine(point.X.ToString(NumberFormatToUse) + "\t" + y + "\t" + point.Y);
                }
            }
        }

        private static void WriteXYData(DataParameters dataParameters, List<XYDataSet> simplifiedDataSets, StreamWriter sw)
        {
            int counter = 0;
            string lineToWrite = "Time\t";
            for (int i = 0; i < simplifiedDataSets.Count; i++)
            {
                if (i < simplifiedDataSets.Count - 1)
                {
                    lineToWrite += NormalizeDataSetName(simplifiedDataSets[i].SourceFileName) + '\t';
                }
                else
                {
                    lineToWrite += NormalizeDataSetName(simplifiedDataSets[i].SourceFileName);
                }
            }
            sw.WriteLine(lineToWrite);

            while (counter < simplifiedDataSets[0].Points.Length)
            {
                if (dataParameters.InferXColumn)
                    lineToWrite = counter.ToString() + '\t';
                else
                {
                    lineToWrite = simplifiedDataSets[0].Points[counter].X.ToString(NumberFormatToUse) + '\t';
                }

                for (int i = 0; i < simplifiedDataSets.Count; i++)
                {
                    if (i < simplifiedDataSets.Count - 1)
                    {
                        lineToWrite += simplifiedDataSets[i].Points[counter].Y.ToString(NumberFormatToUse) + '\t';
                    }
                    else
                    {
                        lineToWrite += simplifiedDataSets[i].Points[counter].Y.ToString(NumberFormatToUse);
                    }
                }
                counter++;
                sw.WriteLine(lineToWrite);
            }
        }

        private static string NormalizeDataSetName(string sourceFileName)
        {
            sourceFileName = Path.GetFileNameWithoutExtension(sourceFileName);
            sourceFileName = sourceFileName.Replace('_', ' ');
            sourceFileName = sourceFileName.Replace(" prot ", " % ");
            return sourceFileName;
        }

        private static bool ShowDialogGetFilePathsToProcess()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Ascii files (*.asc, *.txt, *.csv)|*.asc;*.txt;*.csv";
                ofd.Multiselect = true;

                var dlgResult = ofd.ShowDialog();
                if (dlgResult != DialogResult.OK)
                    return false;

                fileFullPaths = ofd.FileNames;
            }

            return true;
        }

        private static int? ShowDialogGetCompactionRatio(out int? numberOfPointsToSkip)
        {
            Form1 form = new Form1();
            Application.Run(form);
            if (form.ShouldProceed)
            {
                numberOfPointsToSkip = form.NumberOfPointsToSkip;
                return form.DataCompactionRatio;
            }
            else
            {
                numberOfPointsToSkip = null;
                return null;
            }
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }



    }
}
