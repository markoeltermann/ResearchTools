using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary;
using System.Globalization;

namespace SpectrumFolderSimplifier.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {

        private NumberFormatInfo NumberFormat = CultureInfo.InvariantCulture.NumberFormat;

        public MainWindowViewModel()
        {
            SimplifyFolderCommand = new RelayCommand(SimplifyFolder);
        }

        private string _DataFolderPath;
        public string DataFolderPath
        {
            get { return _DataFolderPath; }
            set
            {
                _DataFolderPath = value;
                OnPropertyChanged(nameof(DataFolderPath));
            }
        }

        private double _ProcessedFileRelativeAmount;
        public double ProcessedFileRelativeAmount
        {
            get { return _ProcessedFileRelativeAmount; }
            set
            {
                _ProcessedFileRelativeAmount = value;
                OnPropertyChanged(nameof(ProcessedFileRelativeAmount));
            }
        }


        public RelayCommand SimplifyFolderCommand { get; }

        private bool isSimplifying;
        private async void SimplifyFolder(object obj)
        {
            if (isSimplifying)
                return;

            isSimplifying = true;
            try
            {
                if (DataFolderPath == null)
                    return;
                if (!Directory.Exists(DataFolderPath))
                    return;

                var firstFile = Directory.GetFiles(DataFolderPath, "0.txt").FirstOrDefault();
                if (firstFile == null)
                    return;

                var firstFileData = XYAsciiFileReader.ReadFileFirstColumn(firstFile);

                var outputPath = Path.GetDirectoryName(DataFolderPath) + "\\" + Path.GetFileName(DataFolderPath) + "_simplified";
                Directory.CreateDirectory(outputPath);

                WriteValuesToFile(firstFileData.Select(xy => xy.X), outputPath + "\\xvalues.txt");

                var spectrumFiles = Directory.GetFiles(DataFolderPath);
                int currentProcessedFileCount = 0;

                var progress = new Progress<int>(i =>
                {
                    currentProcessedFileCount++;
                    ProcessedFileRelativeAmount = (double)currentProcessedFileCount / spectrumFiles.Length;
                });
                var syncObject = new object();
                await Task.Run(() => Parallel.ForEach(spectrumFiles, filePath =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (!int.TryParse(fileName, out int index))
                        return;
                    string fileContents;
                    lock (syncObject)
                        fileContents = File.ReadAllText(filePath);
                    var fileData = XYAsciiFileReader.ReadFileContentsFirstColumnAsArray(false, false, fileContents);
                    lock (syncObject)
                        WriteValuesToFile(fileData.Select(xy => xy.Y), outputPath + "\\" + fileName + ".txt");
                    ((IProgress<int>)progress).Report(0);
                }));

                ProcessedFileRelativeAmount = 0;
            }
            finally
            {
                isSimplifying = false;
            }
        }

        private void WriteValuesToFile(IEnumerable<double> values, string filePath)
        {
            using (var sw = new StreamWriter(filePath, false))
            {
                foreach (var value in values)
                {
                    sw.WriteLine(value.ToString(NumberFormat));
                }
            }
        }

    }
}
