using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LabDataViewer.ViewModel.Commands
{
    public class CreateParamsFileCommand : ICommand
    {

        private const string DefaultParamsFilePath = @"C:\Data\Kool\Bakatöö\Mälupulgalt\general\defaultparams.txt";

        private MainWindowViewModel viewModel;

        public CreateParamsFileCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return viewModel.IsSignalSpectrumDirectorySelected && !viewModel.DoesParamsFileExist;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                viewModel.IsSignalSpectrumDirectorySelectedChanged += value;
                viewModel.DoesParamsFileExistChanged  += value;
            }
            remove
            {
                viewModel.IsSignalSpectrumDirectorySelectedChanged -= value;
                viewModel.DoesParamsFileExistChanged -= value;
            }
        }

        public void Execute(object parameter)
        {
            if (viewModel.SelectedExplorerItem == null)
                return;
            var path = viewModel.SelectedExplorerItem.FullPath;
            var paramsFilePath = path.EndsWith("\\") ? path + "params.txt" : path + "\\params.txt";
            try
            {
                File.Copy(DefaultParamsFilePath, paramsFilePath, false);
                viewModel.ReopenSelectedDirectory();
            }
            catch (IOException)
            {

            }
        }

        #endregion
    }
}
