using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LabDataViewer.ViewModel
{

    public enum DirectoryType
    {
        None = 0,
        MainLevelDirectory,
        SignalSpectrumDirectory,
        OtherDirectory,
        ParamsFile,
        AnalyzedDataFile, 
        PlotDataFile,
    }

    public class DataExplorerItemViewModel : ViewModelBase
    {

        public DataExplorerItemViewModel(string directoryPath)
            : this(directoryPath, null, DirectoryType.MainLevelDirectory)
        {
            if (IsSpectrumDirectory())
            {
                DirectoryType = DirectoryType.SignalSpectrumDirectory;
                CreateSpectrumDirectorySubItems(directoryPath);
            }
            else
            {
                DirectoryType = DirectoryType.OtherDirectory;
            }
        }

        public DataExplorerItemViewModel(string directoryPath, DirectoryType type)
            : this(directoryPath, null, type)
        { }

        public DataExplorerItemViewModel(string directoryPath, IList<string> subDirectoryPaths, DirectoryType type)
        {
            //if (string.IsNullOrEmpty(directoryPath))
            //    throw new ArgumentException();
            Contract.Requires(!string.IsNullOrEmpty(directoryPath));

            this.FullPath = directoryPath;
            //this.IsFile = isFile;

            string temp = directoryPath;
            if (temp.EndsWith("\\"))
                temp = temp.Substring(0, temp.Length - 1);

            string dirName = temp.Substring(temp.LastIndexOf("\\") + 1);
            Title = dirName;

            //if (isFile)
            //    DirectoryType = ViewModel.DirectoryType.AsciiDataFile;
            //else
            //DirectoryType = DirectoryType.MainLevelDirectory;
            this.DirectoryType = type;

            if (subDirectoryPaths != null)
            {
                SubDirectoryInfos = new List<DataExplorerItemViewModel>(subDirectoryPaths.Count);
                foreach (var subDirectoryPath in subDirectoryPaths)
                {
                    SubDirectoryInfos.Add(new DataExplorerItemViewModel(subDirectoryPath));
                }
            }
            else
            {
                SubDirectoryInfos = new List<DataExplorerItemViewModel>();
            }

            if (type == DirectoryType.MainLevelDirectory)
            {
                CreateMainLevelDirectorySubItems(directoryPath);
            }
        }

        private void CreateSpectrumDirectorySubItems(string directoryPath)
        {
            //var textFileFullNames = Directory.GetFiles(directoryPath, "*.txt");
            //SubDirectoryInfos = new List<DirectoryInfoViewModel>(textFileFullNames.Length);
            //foreach (var textFile in textFileFullNames)
            //{
            //    SubDirectoryInfos.Add(new DirectoryInfoViewModel(textFile, true));
            //}
            var paramsFileFullNames = Directory.GetFiles(directoryPath, "params.txt");
            var analyzedDataFileFullNames = Directory.GetFiles(directoryPath, "analyzed.txt");
            if (paramsFileFullNames.Length != 0 || analyzedDataFileFullNames.Length != 0)
            {
                SubDirectoryInfos = new List<DataExplorerItemViewModel>();
                if (paramsFileFullNames.Length != 0)
                    SubDirectoryInfos.Add(new DataExplorerItemViewModel(paramsFileFullNames[0], DirectoryType.ParamsFile));
                if (analyzedDataFileFullNames.Length != 0)
                    SubDirectoryInfos.Add(new DataExplorerItemViewModel(analyzedDataFileFullNames[0], DirectoryType.AnalyzedDataFile));
            }
        }

        private void CreateMainLevelDirectorySubItems(string directoryPath)
        {
            var spectrumFiles = Directory.GetFiles(directoryPath, "*sp.txt");
            foreach (var spectrumFile in spectrumFiles)
            {
                SubDirectoryInfos.Add(new DataExplorerItemViewModel(spectrumFile, ViewModel.DirectoryType.PlotDataFile));
            }
        }

        public string Title { get; private set; }

        public string FullPath { get; private set; }

        private bool IsSpectrumDirectory()
        {
            if (Title.StartsWith("gt"))
                return true;
            if (Title.EndsWith("gt"))
                return true;

            return false;
        }

        //public bool IsFile { get; private set; }

        public List<DataExplorerItemViewModel> SubDirectoryInfos { get; private set; }

        private bool _IsSelected;
        public const string IsSelectedPropertyName = "IsSelected";
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnIsSelectedChanged();
            }
        }

        public DirectoryType DirectoryType { get; private set; }


        public event EventHandler IsSelectedChanged;

        protected virtual void OnIsSelectedChanged()
        {
            OnPropertyChanged(IsSelectedPropertyName);
            var handler = IsSelectedChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

    }
}
