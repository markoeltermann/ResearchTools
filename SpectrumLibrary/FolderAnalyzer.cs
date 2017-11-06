using MathNet.Numerics.Integration;
using MathNet.Numerics.Interpolation;
using SpectrumLibrary;
using SpectrumLibrary.Spectrum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpectrumLibrary
{
    public class FolderAnalyzer
    {

        private string[] filePaths;
        //private Spectrum[] spectrums;
        private object syncRoot;
        private List<DataPoint> points;
        private AutoResetEvent endWaitHandle;
        private NumberFormatInfo numberFormat;
        private SpectrumMetadata spectrumRegions;

        //public bool CacheInMemory
        //{
        //    get;
        //    set;
        //}

        public FolderAnalyzer()
        {
            syncRoot = new object();
            points = new List<DataPoint>();
            endWaitHandle = new AutoResetEvent(false);
            numberFormat = CultureInfo.InvariantCulture.NumberFormat;
        }

        public async Task AnalyzeAsync(string path)
        {
            bool isZipped = false;
            var folderName = path.Split('\\').Last();
            var zipFilePath = Path.Combine(path, folderName + ".zip");
            if (File.Exists(zipFilePath))
            {
                isZipped = true;
                //var zipArchive = ZipFile.OpenRead(zipFilePath.First());
            }

            spectrumRegions = SpectrumMetadata.ReadFromFile(path + "\\params.txt");
            var bc = new BlockingCollection<(int index, string data, double[] xData)>();
            var task = Task.Run(() => Parallel.ForEach(bc.GetConsumingEnumerable(), AnalyzeSpectrum));

            if (isZipped)
            {
                await Task.Run(() => ReadZipFile(zipFilePath, bc));
            }
            else
            {
                filePaths = Directory.GetFiles(path, "*.txt");
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string spectrumFilePath = filePaths[i];
                    int? index = GetSpectrumIndexFromPath(spectrumFilePath);
                    if (index.HasValue)
                    {
                        //Spectrum spectrum = new Spectrum(index.Value, XYAsciiFileReader.ReadFileFirstColumnAsArray(spectrumFilePath, false, spectrumRegions.UseCommaFix));
                        //bc.Add(spectrum);
                        bc.Add((index.Value, File.ReadAllText(spectrumFilePath), null));
                    }
                }
            }
            bc.CompleteAdding();

            await task;

            points.Sort();

            WriteResultsInFile(path + @"\analyzed.txt");

            DoCleanup();
        }

        private void ReadZipFile(string zipFilePath, BlockingCollection<(int index, string data, double[] xData)> bc)
        {
            using (var zipArchive = ZipFile.OpenRead(zipFilePath))
            {
                var xData = ReadXDataFromZipArchiveIfPresent(zipArchive);

                foreach (var entry in zipArchive.Entries)
                {
                    if (entry.Name.EndsWith(".txt"))
                    {
                        var index = GetSpectrumIndexFromPath(entry.Name);
                        if (index.HasValue)
                        {
                            using (var entryStream = entry.Open())
                            {
                                using (var sr = new StreamReader(entryStream))
                                {
                                    bc.Add((index.Value, sr.ReadToEnd(), xData));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static double[] ReadXDataFromZipArchiveIfPresent(ZipArchive zipArchive)
        {
            double[] xData = null;
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.Name.EndsWith("xvalues.txt"))
                {

                    using (var entryStream = entry.Open())
                    {
                        using (var sr = new StreamReader(entryStream))
                        {
                            xData = XYAsciiFileReader.ReadFileContentsFirstColumnAsArray(true, false, sr.ReadToEnd()).Select(xy => xy.Y).ToArray();
                        }
                    }
                    break;
                }
            }

            return xData;
        }

        private void WriteResultsInFile(string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                if (spectrumRegions.DefectRegionEnd == 0.0 && spectrumRegions.DefectRegionStart == 0.0)
                {
                    foreach (var point in points)
                    {
                        sw.WriteLine(point.Index + "\t" + point.Value.ReRadiationIntensity.ToString(numberFormat));
                    }
                }
                else
                {
                    foreach (var point in points)
                    {
                        sw.WriteLine(point.Index
                            + "\t" + point.Value.ReRadiationIntensity.ToString(numberFormat)
                            + "\t" + point.Value.DefectRadiationIntensity.ToString(numberFormat)
                            );
                    }
                }
            }
        }

        private void DoCleanup()
        {
            filePaths = null;
            //spectrums = null;
            GC.Collect();
        }

        //private void AnalyzeAsync(string path)
        //{
        //    int? index = GetSpectrumIndexFromPath(path);

        //    if (index.HasValue)
        //    {
        //        var parameters = AnalyzeFile(path);

        //        lock (syncRoot)
        //        {
        //            points.Add(new DataPoint(index.Value, parameters));
        //        }
        //    }
        //}

        private void AnalyzeSpectrum((int index, string dataAsText, double[] xData) data)
        {
            Spectrum spectrum;
            if (data.xData == null)
                spectrum = new Spectrum(data.index, XYAsciiFileReader.ReadFileContentsFirstColumnAsArray(false, spectrumRegions.UseCommaFix, data.dataAsText));
            else
            {
                var yData = XYAsciiFileReader.ReadFileContentsFirstColumnAsArray(true, spectrumRegions.UseCommaFix, data.dataAsText);
                for (int i = 0; i < data.xData.Length; i++)
                {
                    yData[i].X = data.xData[i];
                }
                spectrum = new Spectrum(data.index, yData);
            }

            var parameters = SpectrumAnalysis.AnalyzeSpectrum(spectrum.Points, spectrumRegions);
            lock (syncRoot)
            {
                points.Add(new DataPoint(spectrum.Index, parameters));
            }
        }

        private int? GetSpectrumIndexFromPath(string path)
        {
            string indexString = path.Substring(path.LastIndexOf('\\') + 1);
            indexString = indexString.Substring(0, indexString.LastIndexOf('.'));
            if (!int.TryParse(indexString, out int index))
            {
                return null;
            }
            return index;
        }


        private SpectrumParameters AnalyzeFile(string path)
        {

            var points = XYAsciiFileReader.ReadFileFirstColumnAsArray(path, false, spectrumRegions.UseCommaFix);
            return SpectrumAnalysis.AnalyzeSpectrum(points, spectrumRegions);
        }


        private class DataPoint : IComparable<DataPoint>
        {
            public DataPoint(int index, SpectrumParameters value)
            {
                this.Index = index;
                this.Value = value;
            }

            public int Index;
            public SpectrumParameters Value;

            #region IComparable<DataPoint> Members

            public int CompareTo(DataPoint other)
            {
                return Index.CompareTo(other.Index);
            }

            #endregion
        }

        private class Spectrum
        {

            public Spectrum(int index, XYPoint[] points)
            {
                this.Index = index;
                this.Points = points;
            }

            public int Index;
            public XYPoint[] Points;
        }
    }
}
