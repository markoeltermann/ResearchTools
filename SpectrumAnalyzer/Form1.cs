using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Integration;
using System.Threading;
using SpectrumLibrary;
using SpectrumLibrary.XYData;
using SpectrumLibrary.Spectrum;

namespace SpectrumAnalyzer
{
    public partial class Form1 : Form
    {

        private IList<XYPoint> correctionData;
        private string currentCurveSourcePath;
        private List<XYPoint> baseLinePoints;
        private List<int> peakPointIndexes;

        private XYDataSet<SpectrumMetadata>[] currentChartData;

        public Form1()
        {
            InitializeComponent();
            baseLinePoints = new List<XYPoint>();
            peakPointIndexes = new List<int>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private class ChartPoint
        {
            public ChartPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            public double X
            {
                get;
                private set;
            }
            public double Y
            {
                get;
                private set;
            }
        }

        private void LoadAsciiFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                var dlgResult = dlg.ShowDialog(this);
                if (dlgResult == System.Windows.Forms.DialogResult.OK)
                {
                    LoadChartDataFromFile(dlg.FileName);
                }
            }
        }

        private void LoadChartDataFromFile(string filePath)
        {
            try
            {
                if (currentChartData != null)
                {
                    foreach (var dataSet in currentChartData)
                    {
                        if (dataSet.Name != null)
                            chart1.Series.RemoveByName(dataSet.Name);
                    }
                }

                var factory = new XYDataSetWithSpectrumMetaDataFactory();
                var reader = new XYAsciiFileReader<XYDataSet<SpectrumMetadata>>(filePath);
                currentChartData = reader.ReadMultipleColumnFile(factory);

                for (int i = 0; i < currentChartData.Length; i++)
                {
                    string name;
                    if (!string.IsNullOrEmpty(currentChartData[i].Name))
                        name = currentChartData[i].Name;
                    else
                        name = "Line" + (i + 1);

                    Series series = new Series(name);
                    series.ChartType = SeriesChartType.Line;
                    foreach (var point in currentChartData[i].Points)
                    {
                        series.Points.AddXY(point.X, point.Y);
                    }
                    chart1.Series.Add(series);
                }

                //chart1.DataSource = XYAsciiFileReader.ReadFile(filePath);
                //chart1.DataBind();
                currentCurveSourcePath = filePath;
                //chart1.Series[0].XValueMember = "X";
                //chart1.Series[0].YValueMembers = "Y";
                //chart1.Series[0].ChartType = SeriesChartType.Line;
                RefreshSeriesListBox();
            }
            catch (IOException ioe)
            {
                MessageBox.Show("An error occurred while reading file: " + ioe.Message);
            }
        }

        private void DoFittingButton_Click(object sender, EventArgs e)
        {

            foreach (var dataSet in currentChartData)
            {
                string path = Path.GetDirectoryName(currentCurveSourcePath) + "\\params.txt";
                var regions = SpectrumMetadata.ReadFromFile(path);
                double[] coeffs = Fitting.DoFittingReturnCoeffs(dataSet.Points, regions);
                AddFittedCurveToGraph(coeffs, regions, "fitted_" + dataSet.Name, dataSet.Points);
            }

            RefreshSeriesListBox();
        }

        private void AddFittedCurveToGraph(double[] coefficents, SpectrumMetadata regions, string seriesName, IList<XYPoint> sourcePoints)
        {
            Series series = chart1.Series.FindByName(seriesName);
            if (series == null)
            {
                series = new Series(seriesName);
                series.ChartType = SeriesChartType.Line;
                chart1.Series.Add(series);
            }
            series.Points.Clear();
            IList<XYPoint> points = sourcePoints;
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    if (point.X < regions.FittingRegionA1 || point.X > regions.FittingRegionB2)
                        continue;
                    double value = coefficents[0];
                    for (int j = 1; j < coefficents.Length; j++)
                    {
                        value += Math.Pow(point.X, j) * coefficents[j];
                    }
                    series.Points.AddXY(point.X, value);
                }
            }
        }

        private void YAxisUpperBoundTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var upperBound = Utilities.TryParseDouble(YAxisUpperBoundTextBox.Text);
                if (upperBound.HasValue)
                {
                    if (upperBound.Value > chart1.ChartAreas[0].AxisY.Minimum)
                        chart1.ChartAreas[0].AxisY.Maximum = upperBound.Value;
                }
                else
                    chart1.ChartAreas[0].AxisY.Maximum = double.NaN;
            }
        }

        private void RemoveFittedCurveButton_Click(object sender, EventArgs e)
        {
            int index = chart1.Series.IndexOf("fitted");
            if (index != -1)
            {
                chart1.Series.RemoveAt(index);
            }
        }

        private void YAxisLowerBoundTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var lowerBound = Utilities.TryParseDouble(YAxisLowerBoundTextBox.Text);
                if (lowerBound.HasValue)
                {
                    if (lowerBound.Value < chart1.ChartAreas[0].AxisY.Maximum)
                        chart1.ChartAreas[0].AxisY.Minimum = lowerBound.Value;
                }
                else
                {
                    chart1.ChartAreas[0].AxisY.Minimum = double.NaN;
                }
            }
        }

        private void IntervalTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var interval = Utilities.TryParseDouble(IntervalTextBox.Text);
                if (interval.HasValue)
                {
                    if (interval.Value > 0)
                        chart1.ChartAreas[0].AxisX.Interval = interval.Value;
                }
                else
                {
                    chart1.ChartAreas[0].AxisX.Interval = double.NaN;
                }
            }
        }

        private void AnalyzeFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\";
                var dlgResult = dlg.ShowDialog(this);
                if (dlgResult == System.Windows.Forms.DialogResult.OK)
                {
                    //var filePaths = Directory.GetFiles(dlg.SelectedPath, "*.txt");
                    //using (StreamWriter sw = new StreamWriter(dlg.SelectedPath + @"\analyzed.txt", false))
                    //{


                    //    foreach (var file in filePaths)
                    //    {

                    //        var parameters = AnalyzeSpectrum(file);
                    //        //sw.WriteLine(parameters.DefectRadiationIntensity.ToString() + "\t" + parameters.ReRadiationIntensity.ToString());
                    //        sw.WriteLine(parameters.DefectRadiationIntensity.ToString() + "\t" + parameters.ReRadiationIntensity.ToString());
                    //    }
                    //}
                    FolderAnalyzer fa = new FolderAnalyzer();
                    fa.CacheInMemory = true;
                    fa.AnalyzeAsync(dlg.SelectedPath);
                    LoadChartDataFromFile(dlg.SelectedPath + @"\analyzed.txt");
                }
            }
        }


        private void LoadCorrectionDataButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                var dlgResult = dlg.ShowDialog(this);
                if (dlgResult == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        //chart1.Series[0].Points.Clear();
                        correctionData = XYAsciiFileReader.ReadFile(dlg.FileName)[0];

                        //chart1.Series[0].XValueMember = "X";
                        //chart1.Series[0].YValueMembers = "Y";
                        //chart1.Series[0].ChartType = SeriesChartType.Line;
                    }
                    catch (IOException ioe)
                    {
                        MessageBox.Show("An error ocurred while reading file: " + ioe.Message);
                    }
                }
            }
        }

        private void CorrectDataButton_Click(object sender, EventArgs e)
        {
            IList<XYPoint> points = chart1.DataSource as IList<XYPoint>;
            if (points != null && correctionData != null && points.Count == correctionData.Count)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = new XYPoint(points[i].X, (points[i].Y - 400) / (correctionData[i].Y));
                }
                chart1.DataBind();
            }
            else
                MessageBox.Show("Correction failed");
        }

        private void chart1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadChartDataFromFile(filePaths[0]);
            }
        }

        private void chart1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if (AddBaselinePointsCheckBox.Checked)
            {
                var hitTestResult = chart1.HitTest(e.X, e.Y);
                if (hitTestResult.PointIndex != -1 && hitTestResult.Series.Name == chart1.Series[0].Name)
                {
                    var point = hitTestResult.Series.Points[hitTestResult.PointIndex];

                    AddBaseLinePoint(point.XValue, point.YValues[0]);
                }
            }
            else if (AddPeakPointsCheckBox.Checked)
            {
                var hitTestResult = chart1.HitTest(e.X, e.Y);
                if (hitTestResult.PointIndex != -1 && hitTestResult.Series.Name == chart1.Series[0].Name)
                {
                    var point = hitTestResult.Series.Points[hitTestResult.PointIndex];

                    AddPeakPoint(point.XValue, point.YValues[0], hitTestResult.PointIndex);
                }
            }
        }

        private void AddBaseLinePoint(double x, double y)
        {
            Series series = chart1.Series.FindByName("baselinepoints");
            if (series == null)
            {
                series = new Series("baselinepoints");
                chart1.Series.Add(series);
                series.ChartType = SeriesChartType.Point;
            }

            series.Points.AddXY(x, y);
            baseLinePoints.Add(new XYPoint(x, y));
        }

        private void AddPeakPoint(double x, double y, int index)
        {
            Series series = chart1.Series.FindByName("peakpoints");
            if (series == null)
            {
                series = new Series("peakpoints");
                chart1.Series.Add(series);
                series.ChartType = SeriesChartType.Point;
            }

            series.Points.AddXY(x, y);
            peakPointIndexes.Add(index);
        }

        private void RemoveBaselineButton_Click(object sender, EventArgs e)
        {
            chart1.Series.RemoveByName("baseline");
            chart1.Series.RemoveByName("baselinepoints");
            baseLinePoints.Clear();
        }

        private void CalculateBaselineButton_Click(object sender, EventArgs e)
        {
            if (baseLinePoints.Count > 3)
            {
                var parameters = SpectrumMetadata.FullRegions;
                var order = Utilities.TryParseDouble(BaseLineOrderTextBox.Text);
                if (order.HasValue)
                    parameters.FittingOrder = (int)order.Value;
                else
                    parameters.FittingOrder = 4;
                var coeffs = Fitting.DoFittingReturnCoeffs(baseLinePoints, parameters);
                AddFittedCurveToGraph(coeffs, parameters, "baseline", baseLinePoints);
            }
        }

        private void CalculatePeakValuesButton_Click(object sender, EventArgs e)
        {
            Series mainSeries = chart1.Series[0];
            Series baselineSeries = chart1.Series.FindByName("baseline");
            if (mainSeries == null || baselineSeries == null)
                return;

            string text = "";
            peakPointIndexes.Sort();
            foreach (var index in peakPointIndexes)
            {
                double baseLineValue = baselineSeries.Points[index].YValues[0];
                double value = mainSeries.Points[index].YValues[0];
                value = value / baseLineValue - 1;
                text += value.ToString() + Environment.NewLine;
            }

            Clipboard.Clear();
            Clipboard.SetText(text);
        }

        private void RemovePeakPointsButton_Click(object sender, EventArgs e)
        {
            peakPointIndexes.Clear();
            chart1.Series.RemoveByName("peakpoints");
        }

        #region SeriesListBox

        private void SeriesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBoxItem item = (CheckedListBoxItem)SeriesListBox.Items[e.Index];
            item.Series.Enabled = e.NewValue == CheckState.Checked;
        }

        private void RefreshSeriesListBox()
        {
            SeriesListBox.Items.Clear();
            SeriesListBox.DisplayMember = "";
            foreach (var series in chart1.Series)
            {
                CheckedListBoxItem item = new CheckedListBoxItem();
                item.Series = series;
                SeriesListBox.Items.Add(item, series.Enabled);
            }
        }

        private class CheckedListBoxItem
        {
            public Series Series;
            public override string ToString()
            {
                if (Series != null)
                    return Series.Name;
                else
                    return base.ToString();
            }
        }

        #endregion

    }
}
