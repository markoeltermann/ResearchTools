using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LabDataViewer.ViewModel.Commands
{
    public class ResetPlotAxesCommand : ICommand
    {

        private MainWindowViewModel viewModel;

        public ResetPlotAxesCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        private void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainWindowViewModel.IsPlotFileActivePropertyName)
            {
                OnCanExecuteChanged();
            }
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.IsPlotFileActive;
        }

        protected void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            foreach (var axis in viewModel.PlotModel.Axes)
            {
                axis.Minimum = axis.DataMinimum;
                axis.Maximum = axis.DataMaximum;
            }
            bool reloadData = false;
            if (parameter != null && parameter is bool)
                reloadData = (bool)parameter;
            viewModel.PlotModel.ResetAllAxes();
            viewModel.PlotModel.InvalidatePlot(reloadData);
            viewModel.TryDeleteCurrentPlotAxesConfiguration();
        }
    }
}
