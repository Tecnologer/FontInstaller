using FontInstaller.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace FontInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private State currentState;
        private readonly FontCore fontCore;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            fontCore = new FontCore();
            DataContext = this;
        }
        public State CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if(currentState != value)
                {
                    currentState = value;
                    OnPropertyChanged("CurrentState");
                }
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = currentState == State.Processing;
        }

        private void InstallCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TxtSourcePath != null && !string.IsNullOrEmpty(TxtSourcePath.Text);
        }

        private void SearchZipCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SearchZipCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Archivo Comprimido", // Default file name
                DefaultExt = ".zip, .rar, .tar, .7z", // Default file extension
                Filter = "Archivo Comprimido|*.zip;*.rar;*.tar;*.7z" // Filter files by extension
            };

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                TxtSourcePath.Text = dlg.FileName;
            }
        }

        private void InstallCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentState = State.Processing;
            var worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += InstallAsync;
            worker.RunWorkerCompleted += InstallCompleted;
            worker.ProgressChanged += InstallationProgress;
            worker.RunWorkerAsync(TxtSourcePath.Text);
        }

        private void InstallationProgress(object sender, ProgressChangedEventArgs e)
        {
            var progress = e.UserState as ReportProgress;
            var progresLabel = progress.Label;

            if (string.IsNullOrEmpty(progresLabel))
            {
                switch (progress.State)
                {
                    case InstalationState.Installing:
                        progresLabel = string.Format("Instalando fuente {0} de {1}", progress.Current, progress.Total);
                        break;
                    case InstalationState.OpenFile:
                        progresLabel = "Descomprimiendo archivo...";
                        break;
                    case InstalationState.SearchFont:
                        progresLabel = "Buscando fuentes";
                        break;
                }
            }
            Dispatcher.BeginInvoke((Action)delegate
            {
                progressBar.Value = progress.Current;
                progressBar.Maximum = progress.Total;
                ProgressTxt.Text = progresLabel;
            });
        }

        private void InstallCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentState = State.Normal;
        }

        private void InstallAsync(object sender, DoWorkEventArgs e)
        {
            var source = e.Argument as string;
            fontCore.InstallFonts(source, sender as BackgroundWorker);
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentState = State.Normal;
        }

        private void SearchFolderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK) return;

                TxtSourcePath.Text = dialog.SelectedPath;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtSourcePath.Text = "";
            TxtSourcePath.Focus();
        }
    }

    public enum State
    {
        Normal,
        Processing,
        Dropping
    }
}
