using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LabDataViewer.ViewModel.Commands;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SpectrumLibrary.XYData;
using LabDataViewer.Model;
using Newtonsoft.Json;
using System.Reflection;

namespace LabDataViewer.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private bool isPlotOpeningInProgress;
        private string dataFolderPath;

        public MainWindowViewModel()
        {
            DirectoryInfos = new ObservableCollection<DataExplorerItemViewModel>();
            InitializeDataFolderPath();
            foreach (var directory in Directory.GetDirectories(dataFolderPath))
            {
                var directoryInfoViewModel = new DataExplorerItemViewModel(directory, Directory.GetDirectories(directory), DirectoryType.MainLevelDirectory);
                AddDirectoryInfoEventHandlers(directoryInfoViewModel);
                DirectoryInfos.Add(directoryInfoViewModel);
            }

            IsPlotFileActive = true;
            PlotModel = new PlotModel();
            foreach (var axis in PlotModel.Axes)
            {
                axis.MajorGridlineStyle = LineStyle.Solid;
            }
            PlotModel.ResetAllAxes();
            PlotModel.InvalidatePlot(true);

            SyncOpenTextFileContentsToDiskCommand = new SyncOpenTextFileContentsToDiskCommand(this);
            CreateParamsFileCommand = new CreateParamsFileCommand(this);
            AnalyzeSignalSpectrumFolderCommand = new AnalyzeSignalSpectrumFolderCommand(this);
            ResetPlotAxesCommand = new ResetPlotAxesCommand(this);
        }

        private void InitializeDataFolderPath()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            var exeFolderPath = Path.GetDirectoryName(exePath);
            var configFilePath = exeFolderPath + @"\config.json";
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                var config = JsonConvert.DeserializeObject<SoftwareConfiguration>(json);
                dataFolderPath = config.DataFolderPath;
            }
            else
            {
                this.dataFolderPath = @"C:\Data\Research\Mälupulgalt";
                var config = new SoftwareConfiguration();
                config.DataFolderPath = dataFolderPath;
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configFilePath, json);
            }
        }

        private void AddDirectoryInfoEventHandlers(DataExplorerItemViewModel directoryInfoViewModel)
        {
            directoryInfoViewModel.IsSelectedChanged += directoryInfoViewModel_IsSelectedChanged;
            foreach (var subDirectoryInfoViewModel in directoryInfoViewModel.SubDirectoryInfos)
            {
                AddDirectoryInfoEventHandlers(subDirectoryInfoViewModel);
            }
        }

        private void Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            if (!isPlotOpeningInProgress)
                SavePlotModelAxesConfiguration();
        }

        private void axis_TransformChanged(object sender, EventArgs e)
        {
            if (!isPlotOpeningInProgress)
                SavePlotModelAxesConfiguration();
        }

        #region Plot conf read/write
        private void SavePlotModelAxesConfiguration()
        {
            if (SelectedExplorerItem != null)
            {
                var path = SelectedExplorerItem.FullPath + ".config";
                //List<string> configuration = new List<string>();
                Dictionary<string, AxisData> configuration = new Dictionary<string, AxisData>();
                foreach (var axis in PlotModel.Axes)
                {
                    string axisName = null;
                    switch (axis.Position)
                    {
                        case AxisPosition.Bottom:
                            axisName = "X";
                            break;
                        case AxisPosition.Left:
                            axisName = "Y";
                            break;
                        default:
                            continue;
                    }
                    AxisData ad = new AxisData(axisName, axis.ActualMinimum, axis.ActualMaximum);
                    configuration.Add(axisName, ad);
                    //configuration.Add(axisName + ".Min = " + axis.ActualMinimum);
                    //configuration.Add(axisName + ".Max = " + axis.ActualMaximum);
                }
                try
                {
                    //File.WriteAllLines(path, configuration);
                    File.WriteAllText(path, JsonConvert.SerializeObject(configuration));
                }
                catch (Exception)
                {
                }
            }
        }

        private bool TryReadAxesConfigurationToPlotModel(string path)
        {
            path = path + ".config";
            if (File.Exists(path))
            {
                try
                {
                    Dictionary<string, AxisData> configuration = JsonConvert.DeserializeObject<Dictionary<string, AxisData>>(File.ReadAllText(path));
                    foreach (var axisData in configuration.Values)
                    {

                        AxisPosition ap = AxisPosition.None;
                        if (axisData.AxisName == "X")
                            ap = AxisPosition.Bottom;
                        else if (axisData.AxisName == "Y")
                            ap = AxisPosition.Left;
                        else
                            continue;
                        Axis axis = PlotModel.Axes.Where(x => x.Position == ap).FirstOrDefault();
                        if (axis != null)
                        {
                            axis.Minimum = axisData.Minimum;
                            axis.Maximum = axisData.Maximum;
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        public void TryDeleteCurrentPlotAxesConfiguration()
        {
            if (SelectedExplorerItem != null && IsPlotFileActive)
            {
                var path = SelectedExplorerItem.FullPath + ".config";
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion


        public SyncOpenTextFileContentsToDiskCommand SyncOpenTextFileContentsToDiskCommand { get; private set; }
        public CreateParamsFileCommand CreateParamsFileCommand { get; private set; }
        public AnalyzeSignalSpectrumFolderCommand AnalyzeSignalSpectrumFolderCommand { get; private set; }
        public ResetPlotAxesCommand ResetPlotAxesCommand { get; private set; }

        private void directoryInfoViewModel_IsSelectedChanged(object sender, EventArgs e)
        {
            DataExplorerItemViewModel directoryInfoViewModel = (DataExplorerItemViewModel)sender;
            if (directoryInfoViewModel.IsSelected)
            {
                SelectedExplorerItem = directoryInfoViewModel;
            }
            else
            {
                SelectedExplorerItem = null;
            }
        }

        public ObservableCollection<DataExplorerItemViewModel> DirectoryInfos { get; private set; }

        private DataExplorerItemViewModel _SelectedDirectoryInfo;

        public DataExplorerItemViewModel SelectedExplorerItem
        {
            get { return _SelectedDirectoryInfo; }
            set
            {
                _SelectedDirectoryInfo = value;
                ReopenSelectedDirectory();
            }
        }

        public void ReopenSelectedDirectory()
        {
            if (SelectedExplorerItem != null)
            {
                switch (SelectedExplorerItem.DirectoryType)
                {
                    case DirectoryType.MainLevelDirectory:
                        TryFindLogFileSetInfoText(SelectedExplorerItem.FullPath);
                        IsSignalSpectrumDirectorySelected = false;
                        break;
                    case DirectoryType.SignalSpectrumDirectory:
                    case DirectoryType.ParamsFile:
                        OpenSignalSpectrumDirectory(SelectedExplorerItem);
                        break;
                    case DirectoryType.AnalyzedDataFile:
                    case DirectoryType.PlotDataFile:
                        OpenPlotFile(SelectedExplorerItem.FullPath, null);
                        break;
                    default:
                        OpenTextFileInEditor(null);
                        IsSignalSpectrumDirectorySelected = false;
                        break;
                }
            }
        }

        private void OpenSignalSpectrumDirectory(DataExplorerItemViewModel directoryInfoViewModel)
        {
            string paramsFilePath = null;

            if (directoryInfoViewModel.DirectoryType == DirectoryType.SignalSpectrumDirectory)
            {
                var paramsFilePaths = Directory.GetFiles(directoryInfoViewModel.FullPath, "params.txt");
                if (paramsFilePaths.Length == 0)
                {
                    OpenTextFileInEditor(null);
                    DoesParamsFileExist = false;
                }
                else
                {
                    paramsFilePath = paramsFilePaths[0];
                }
            }
            else if (directoryInfoViewModel.DirectoryType == DirectoryType.ParamsFile)
                paramsFilePath = directoryInfoViewModel.FullPath;

            if (paramsFilePath != null)
            {
                OpenTextFileInEditor(paramsFilePath);
                DoesParamsFileExist = true;
            }

            if (directoryInfoViewModel.DirectoryType == DirectoryType.SignalSpectrumDirectory)
                IsSignalSpectrumDirectorySelected = true;
            else
                IsSignalSpectrumDirectorySelected = false;
        }

        private void TryFindLogFileSetInfoText(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                OpenTextFileInEditor(null);
                return;
            }

            var files = Directory.EnumerateFiles(directoryPath);
            var logFilePath = (from file in files
                               where file.EndsWith("log.txt") || file.EndsWith("memo.txt")
                               select file).FirstOrDefault();

            if (logFilePath == null)
            {
                OpenTextFileInEditor(null);
            }
            else
            {
                OpenTextFileInEditor(logFilePath);
            }
        }



        private void OpenTextFileInEditor(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                OpenedFilePath = null;
                CurrentInfoText = null;
                return;
            }
            IsPlotFileActive = false;

            try
            {
                var fileContents = File.ReadAllText(filePath, Encoding.UTF8);
                OpenedFilePath = filePath;
                SetSyncedCurrentInfoText(fileContents);
            }
            catch (IOException)
            {
                OpenedFilePath = null;
                CurrentInfoText = null;
                return;
            }
        }

        private void OpenPlotFile(string path, string title)
        {
            IsSignalSpectrumDirectorySelected = false;
            OpenTextFileInEditor(null);

            try
            {
                isPlotOpeningInProgress = true;

                var data = XYDataSet.ReadFromFile(path, false);
                var series = new LineSeries();
                if (title != null)
                {
                    series.Title = title;
                }
                foreach (var point in data.Points)
                {
                    series.Points.Add(new DataPoint(point.X, point.Y));
                }
                series.MouseDown += Series_MouseDown;
                PlotModel.Series.Clear();
                PlotModel.Series.Add(series);
                PlotModel.ResetAllAxes();

                foreach (var axis in PlotModel.Axes)
                {
                    axis.AxisChanged += Axis_AxisChanged;
                }
                PlotModel.InvalidatePlot(true);
                foreach (var axis in PlotModel.Axes)
                {
                    axis.MajorGridlineStyle = LineStyle.Solid;
                    axis.MajorGridlineColor = OxyColors.Gray;
                }
                if (!TryReadAxesConfigurationToPlotModel(path))
                {
                    ResetPlotAxesCommand.Execute(false);
                }
                else
                {
                    PlotModel.InvalidatePlot(false);
                }


            }
            catch (Exception)
            {
            }
            finally
            {
                isPlotOpeningInProgress = false;
            }
            IsPlotFileActive = true;
        }

        private void Series_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (SelectedExplorerItem != null && SelectedExplorerItem.DirectoryType == DirectoryType.AnalyzedDataFile && e.HitTestResult != null)
            {
                if (e.HitTestResult.Item != null)
                {
                    DataPoint point = (DataPoint)e.HitTestResult.Item;
                    var path = SelectedExplorerItem.FullPath;
                    path = Path.GetDirectoryName(path);
                    path = path + "\\" + ((int)point.X).ToString() + ".txt";
                    if (File.Exists(path))
                    {
                        SelectedExplorerItem.IsSelected = false;
                        OpenPlotFile(path, ((int)point.X).ToString());
                    }
                }
            }
        }

        private bool _IsPlotFileActive;
        public const string IsPlotFileActivePropertyName = "IsPlotFileActive";
        public bool IsPlotFileActive
        {
            get { return _IsPlotFileActive; }
            set
            {
                _IsPlotFileActive = value;
                OnPropertyChanged(IsPlotFileActivePropertyName);
            }
        }


        private string _OpenedFilePath;
        public const string OpenedFilePathPropertyName = "OpenedFilePath";
        public string OpenedFilePath
        {
            get { return _OpenedFilePath; }
            set
            {
                _OpenedFilePath = value;
                OnPropertyChanged(OpenedFilePathPropertyName);
                IsFileOpen = !string.IsNullOrEmpty(_OpenedFilePath);
            }
        }

        private bool _IsFileOpen;
        public const string IsFileOpenPropertyName = "IsFileOpen";
        public bool IsFileOpen
        {
            get { return _IsFileOpen; }
            set
            {
                _IsFileOpen = value;
                OnPropertyChanged(IsFileOpenPropertyName);
            }
        }

        private bool _IsSignalSpectrumDirectorySelected;
        public const string IsSignalSpectrumDirectorySelectedPropertyName = "IsSignalSpectrumDirectorySelected";
        public bool IsSignalSpectrumDirectorySelected
        {
            get { return _IsSignalSpectrumDirectorySelected; }
            set
            {
                _IsSignalSpectrumDirectorySelected = value;
                OnIsSignalSpectrumDirectorySelectedChanged();
            }
        }

        private bool _DoesParamsFileExist;
        public const string DoesParamsFileExistPropertyName = "DoesParamsFileExist";
        public bool DoesParamsFileExist
        {
            get { return _DoesParamsFileExist; }
            set { _DoesParamsFileExist = value; }
        }

        public event EventHandler DoesParamsFileExistChanged;

        protected virtual void OnDoesParamsFileExistChanged()
        {
            var handler = DoesParamsFileExistChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            OnPropertyChanged(DoesParamsFileExistPropertyName);
        }

        public event EventHandler IsSignalSpectrumDirectorySelectedChanged;

        protected virtual void OnIsSignalSpectrumDirectorySelectedChanged()
        {
            var handler = IsSignalSpectrumDirectorySelectedChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            OnPropertyChanged(IsSignalSpectrumDirectorySelectedPropertyName);
        }

        private string _CurrentInfoText;
        public const string CurrentInfoTextPropertyName = "CurrentInfoText";
        public string CurrentInfoText
        {
            get { return _CurrentInfoText; }
            set
            {
                _CurrentInfoText = value;
                OnPropertyChanged(CurrentInfoTextPropertyName);
                IsFileContentsSyncedToDisk = false;
            }
        }

        private void SetSyncedCurrentInfoText(string value)
        {
            _CurrentInfoText = value;
            IsFileContentsSyncedToDisk = true;
            OnPropertyChanged(CurrentInfoTextPropertyName);
        }

        private bool _IsFileContentsSyncedToDisk;
        public const string IsFileContentsSyncedToDiskPropertyName = "IsFileContentsSyncedToDisk";
        public bool IsFileContentsSyncedToDisk
        {
            get { return _IsFileContentsSyncedToDisk; }
            set
            {
                _IsFileContentsSyncedToDisk = value;
                OnIsFileContentsSyncedToDiskChanged();
            }
        }

        private IList<DataPoint> _PlotDataPoints;
        public const string PlotDataPointsPropertyName = "PlotDataPoints";
        public IList<DataPoint> PlotDataPoints
        {
            get { return _PlotDataPoints; }
            set
            {
                _PlotDataPoints = value;
                OnPropertyChanged(PlotDataPointsPropertyName);
            }
        }

        private PlotModel _PlotModel;
        public const string PlotModelPropertyName = "PlotModel";
        public PlotModel PlotModel
        {
            get { return _PlotModel; }
            set
            {
                _PlotModel = value;
                OnPropertyChanged(PlotModelPropertyName);
            }
        }


        public event EventHandler IsFileContentsSyncedToDiskChanged;

        protected virtual void OnIsFileContentsSyncedToDiskChanged()
        {
            var handler = IsFileContentsSyncedToDiskChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            OnPropertyChanged(IsFileOpenPropertyName);
        }
    }
}
