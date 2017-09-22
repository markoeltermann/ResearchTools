using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SpectrumLibrary.XYData;
using System.Globalization;
using SpectrumLibrary.Smoothing;
using SpectrumLibrary.Spectrum;

namespace D20M01Y2015ExperimentDataAnalyzer
{
    class Program
    {

        private const string DataPath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\2015_01_20\";
        private static readonly NumberFormatInfo NumberFormatToUse = CultureInfo.InvariantCulture.NumberFormat;
        private static SpectrumMetadata spectrumMetaData;


        static void Main(string[] args)
        {

            var spectrumFilePaths = Directory.GetFiles(DataPath, "*.txt");
            List<XYDataSet<ExperimentFileInfo>> dataSets = new List<XYDataSet<ExperimentFileInfo>>();
            foreach (var spectrumFilePath in spectrumFilePaths)
            {
                string fileName = Path.GetFileName(spectrumFilePath);
                if (!fileName.Contains('_') && (fileName.StartsWith("sp") || fileName.StartsWith("dec")))
                    dataSets.Add(XYDataSet<ExperimentFileInfo>.ReadFromFile(spectrumFilePath, false));
            }

            var memoFileLines = File.ReadAllLines(DataPath + "memo.txt");
            Dictionary<string, ExperimentFileInfo> experimentFileInfos = new Dictionary<string, ExperimentFileInfo>();
            ExperimentFileInfo previousExperimentFileInfo = null;
            foreach (var memoFileLine in memoFileLines)
            {
                var fileInfos = ExperimentFileInfo.ReadFromMemoFileLine(memoFileLine, previousExperimentFileInfo);
                if (fileInfos != null)
                {
                    foreach (var info in fileInfos)
                    {
                        experimentFileInfos.Add(info.FileName, info);
                    }
                    previousExperimentFileInfo = fileInfos.First();
                }
            }

            foreach (var dataSet in dataSets)
            {
                dataSet.MetaData = experimentFileInfos[dataSet.SourceFileName.Substring(0, dataSet.SourceFileName.IndexOf('.'))];
                dataSet.Name = dataSet.MetaData.Caption;
            }

            SmoothDecayDataSets(dataSets);
            NormalizeDataSets(dataSets);

            spectrumMetaData = SpectrumMetadata.ReadFromFile(DataPath + "params.txt");

            var spectrums10Percent = (from dataSet in dataSets
                                      let fileInfo = dataSet.MetaData
                                      where fileInfo.OxygenPercentage == 10 && !fileInfo.IsDecayFile
                                      orderby fileInfo.MeasurementTemperature
                                      select dataSet).ToList();

            WriteDataSetsToFile(spectrums10Percent, "sp_10%");
            AnalyzeSpectrumsWriteResultToFile(spectrums10Percent, "sp_10%_rad");

            var spectrums100Percent = (from dataSet in dataSets
                                       let fileInfo = dataSet.MetaData
                                       where fileInfo.OxygenPercentage == 100 && !fileInfo.IsDecayFile
                                       orderby fileInfo.MeasurementTemperature
                                       select dataSet).ToList();
            WriteDataSetsToFile(spectrums100Percent, "sp_100%");
            AnalyzeSpectrumsWriteResultToFile(spectrums100Percent, "sp_100%_rad");

            WriteDataSetsToFile(
                                (from dataSet in dataSets
                                 let fileInfo = dataSet.MetaData
                                 where fileInfo.OxygenPercentage == 10 && fileInfo.IsDecayFile
                                 orderby fileInfo.MeasurementTemperature
                                 select dataSet).ToList(),
                                 "dec_10%"
                                 );

            WriteDataSetsToFile(
                                (from dataSet in dataSets
                                 let fileInfo = dataSet.MetaData
                                 where fileInfo.OxygenPercentage == 100 && fileInfo.IsDecayFile
                                 orderby fileInfo.MeasurementTemperature
                                 select dataSet).ToList(),
                                 "dec_100%"
                                 );

        }

        private static void NormalizeDataSets(List<XYDataSet<ExperimentFileInfo>> dataSets)
        {
            foreach (var dataSet in dataSets)
            {
                double normalizationFactor;
                double delta = 0.0;
                if (dataSet.MetaData.IsDecayFile)
                {
                    normalizationFactor = dataSet.MetaData.InputSlitWidth * dataSet.MetaData.OutputSlitWidth;
                    //normalizationFactor = 1;
                    delta = 3.5;
                }
                else
                {
                    normalizationFactor = dataSet.MetaData.InputSlitWidth * dataSet.MetaData.ExpositionTime;
                }

                for (int i = 0; i < dataSet.PointCount; i++)
                {
                    dataSet.Points[i].Y = (dataSet.Points[i].Y - delta) / 3.5;
                }
            }
        }

        private static void SmoothDecayDataSets(List<XYDataSet<ExperimentFileInfo>> dataSets)
        {
            for (int i = 0; i < dataSets.Count; i++)
            {
                var dataSet = dataSets[i];
                if (dataSet.MetaData.IsDecayFile)
                {
                    dataSets[i] = DataSetSmoothing.SmoothAndCompactDataSet(dataSet, 10, 0);
                }
            }
        }

        private static void WriteDataSetsToFile<T>(List<XYDataSet<T>> dataSets, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(DataPath + fileName + ".txt", false))
            {
                string lineToWrite;
                lineToWrite = "x\t";
                for (int i = 0; i < dataSets.Count; i++)
                {
                    if (i < dataSets.Count - 1)
                    {
                        lineToWrite += dataSets[i].Name + '\t';
                    }
                    else
                    {
                        lineToWrite += dataSets[i].Name;
                    }
                }
                sw.WriteLine(lineToWrite);

                for (int i = 0; i < dataSets[0].Points.Length; i++)
                {
                    lineToWrite = dataSets[0].Points[i].X.ToString(NumberFormatToUse) + '\t';

                    for (int j = 0; j < dataSets.Count; j++)
                    {
                        if (j < dataSets.Count - 1)
                        {
                            lineToWrite += dataSets[j].Points[i].Y.ToString(NumberFormatToUse) + '\t';
                        }
                        else
                        {
                            lineToWrite += dataSets[j].Points[i].Y.ToString(NumberFormatToUse);
                        }
                    }
                    sw.WriteLine(lineToWrite);
                }
            }
        }

        private static void AnalyzeSpectrumsWriteResultToFile(List<XYDataSet<ExperimentFileInfo>> dataSets, string fileName)
        {
            XYDataSet<object> defectRadiationDataSet = new XYDataSet<object>(null, dataSets.Count, null);
            XYDataSet<object> reRadiationDataSet = new XYDataSet<object>(null, dataSets.Count, null);
            defectRadiationDataSet.Name = "defect_radiation";
            reRadiationDataSet.Name = "RE_radiation";
            for (int i = 0; i < dataSets.Count; i++)
            {
                SpectrumParameters sp = SpectrumAnalysis.AnalyzeSpectrum(dataSets[i].Points, spectrumMetaData);
                defectRadiationDataSet.Points[i].Y = sp.DefectRadiationIntensity;
                reRadiationDataSet.Points[i].Y = sp.ReRadiationIntensity;
                defectRadiationDataSet.Points[i].X = dataSets[i].MetaData.MeasurementTemperature;
                reRadiationDataSet.Points[i].X = dataSets[i].MetaData.MeasurementTemperature;
            }
            WriteDataSetsToFile(new List<XYDataSet<object>>(new[] { defectRadiationDataSet, reRadiationDataSet }), fileName);
        }

        private class ExperimentFileInfo
        {

            public int OxygenPercentage;
            public int MeasurementTemperature;
            public string FileName;
            public bool IsDecayFile;
            public int OutputSlitWidth;
            public double ExpositionTime;
            public int InputSlitWidth;

            private ExperimentFileInfo(int oxygenPercentage, int measurementTemperature, string fileName, bool isDecayFile, int outputSlitWidth, double expositionTime, int inputSlitWidth)
            {
                this.OxygenPercentage = oxygenPercentage;
                this.MeasurementTemperature = measurementTemperature;
                this.FileName = fileName;
                this.IsDecayFile = isDecayFile;
                this.OutputSlitWidth = outputSlitWidth;
                this.ExpositionTime = expositionTime;
                this.InputSlitWidth = inputSlitWidth;
            }

            public static ExperimentFileInfo[] ReadFromMemoFileLine(string line, ExperimentFileInfo previousExperimentInfo)
            {
                if (!line.StartsWith("sp"))
                    return null;

                // [0]sp1/dec1 - [1]10% objekt, [2]8 K, [3]väljundpilu 30 um, [4]0.3*25 sek ekspositsioon, [5]sisendpilu 10 um

                var parts = line.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var fileNames = parts[0].Trim().Split('/');
                var oxygenPercentage = int.Parse(parts[1].Substring(0, parts[1].IndexOf('%')));
                var temperature = int.Parse(parts[2].Substring(0, parts[2].IndexOf(" K")));
                int outputSlitWidth;
                if (parts.Length >= 4)
                {
                    outputSlitWidth = int.Parse(parts[3].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                }
                else
                {
                    outputSlitWidth = previousExperimentInfo.OutputSlitWidth;
                }
                int inputSlitWidth;
                if (parts.Length >= 6)
                {
                    inputSlitWidth = int.Parse(parts[5].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                }
                else
                {
                    inputSlitWidth = previousExperimentInfo.InputSlitWidth;
                }
                double expositionTime;
                if (parts.Length >= 5)
                {
                    var expPart = parts[4].Trim();
                    if (expPart.StartsWith("1*25"))
                        expositionTime = 1 * 25;
                    else if (expPart.StartsWith("0.3*25"))
                        expositionTime = 0.3 * 25;
                    else
                        throw new Exception();
                }
                else
                {
                    expositionTime = previousExperimentInfo.ExpositionTime;
                }

                return new ExperimentFileInfo[] { new ExperimentFileInfo(oxygenPercentage, temperature, fileNames[0], false, outputSlitWidth, expositionTime, inputSlitWidth),
                                                  new ExperimentFileInfo(oxygenPercentage, temperature, fileNames[1], true, outputSlitWidth, expositionTime, inputSlitWidth) };
            }

            public string Caption
            {
                get
                {
                    return FileName + "_" + OxygenPercentage + "%_" + MeasurementTemperature + "K";
                }
            }

        }
    }

}
