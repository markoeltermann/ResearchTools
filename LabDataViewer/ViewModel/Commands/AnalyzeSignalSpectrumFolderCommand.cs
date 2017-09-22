using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SpectrumLibrary;

namespace LabDataViewer.ViewModel.Commands
{
    public class AnalyzeSignalSpectrumFolderCommand : ICommand
    {
        private MainWindowViewModel viewModel;
        private bool canExecuteInternal;

        public AnalyzeSignalSpectrumFolderCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            canExecuteInternal = true;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return viewModel.IsSignalSpectrumDirectorySelected && viewModel.DoesParamsFileExist && canExecuteInternal;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                viewModel.IsSignalSpectrumDirectorySelectedChanged += value;
                viewModel.DoesParamsFileExistChanged += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                viewModel.IsSignalSpectrumDirectorySelectedChanged -= value;
                viewModel.DoesParamsFileExistChanged -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        private event EventHandler CanExecuteChangedInternal;

        protected void OnCanExecuteChangedInternal()
        {
            CanExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        public async void Execute(object parameter)
        {
            canExecuteInternal = false;
            OnCanExecuteChangedInternal();
            try
            {
                FolderAnalyzer fa = new FolderAnalyzer();
                //fa.CacheInMemory = true;
                await fa.AnalyzeAsync(viewModel.SelectedExplorerItem.FullPath);

            }
            finally
            {
                canExecuteInternal = true;
                try
                {
                    OnCanExecuteChangedInternal();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion
    }
}
