using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SpectrumFolderSimplifier.ViewModel;
using System.IO;

namespace SpectrumFolderSimplifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = (MainWindowViewModel)DataContext;

            Loaded += MainWindow_Loaded;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data is string[] paths)
            {
                if (paths.Length == 1)
                {
                    if (Directory.Exists(paths[0]))
                    {
                        viewModel.DataFolderPath = paths[0];
                    }
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
