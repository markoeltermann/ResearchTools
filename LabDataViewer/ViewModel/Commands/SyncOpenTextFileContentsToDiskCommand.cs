using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace LabDataViewer.ViewModel.Commands
{
    public class SyncOpenTextFileContentsToDiskCommand : ICommand
    {

        private MainWindowViewModel viewModel;

        public SyncOpenTextFileContentsToDiskCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return !viewModel.IsFileContentsSyncedToDisk && viewModel.IsFileOpen;
        }

        public event EventHandler CanExecuteChanged
        {
            add { viewModel.IsFileContentsSyncedToDiskChanged += value; }
            remove { viewModel.IsFileContentsSyncedToDiskChanged -= value; }
        }

        public void Execute(object parameter)
        {
            var filePath = viewModel.OpenedFilePath;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            var newFileContents = viewModel.CurrentInfoText;
            try
            {
                File.WriteAllText(filePath, newFileContents, Encoding.Default);
                viewModel.IsFileContentsSyncedToDisk = true;
            }
            catch (IOException)
            {

            }
        }

        #endregion
    }
}
