﻿using FontInstaller.Core;
using FontInstaller.Core.Helpers;
using FontInstaller.Dialogs;
using Syroot.Windows.IO;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FontInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string lastFolderSelected;
        private State currentState;
        private readonly FontCore fontCore;        
        public event PropertyChangedEventHandler PropertyChanged;
        BackgroundWorker worker;
        public MainWindow()
        {
            Logger.Log("Application stated", Environment.OSVersion);
            InitializeComponent();
            fontCore = new FontCore();
            DataContext = this;

            Title = string.Format("{0} ({1})", Title, _version);
#if DEBUG
            TxtSourcePath.Text = @"D:\Download\fonts.zip";
#endif
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
                DefaultExt = ".zip", // Default file extension
                Filter = "Archivo Comprimido|*.zip;" // Filter files by extension
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
            fontCore.CancelDecryption = false;
            fontCore.IsPasswordSetted = false;
            fontCore.ZipPassword = null;

            CurrentState = State.Processing;
            worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
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
                    case InstalationState.PasswordRequired:
                        var dialog = new DialogPassword()
                        {
                            Owner = this
                        };

                        if(dialog.ShowDialog() == true)
                        {
                            fontCore.ZipPassword = dialog.PasswordInput.Password.ToString();
                            fontCore.IsPasswordSetted = true;
                        }
                        else
                        {
                            fontCore.CancelDecryption = true;
                        }
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
            
            var tuple = e.Result as WorkerResult;
            var header = tuple.HasError ? "Error" : "Proceso finalizado";
            var typeBox = tuple.HasError ? MessageBoxImage.Error : MessageBoxImage.Information;

            MessageBox.Show(this, tuple.Msg,
                                        header,
                                        MessageBoxButton.OK,
                                        typeBox);
            
        }

        private void InstallAsync(object sender, DoWorkEventArgs e)
        {
            var source = e.Argument as string;
            if (!Compressed.IsCompressedFile(source))
            {
                fontCore.InstallFontsFromFolder(source, sender as BackgroundWorker, e);
            }
            else
            {
                fontCore.InstallFontsFromZip(source, sender as BackgroundWorker, e);
            }
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            worker.CancelAsync();
            CurrentState = State.Normal;
        }

        private void SearchFolderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (string.IsNullOrEmpty(lastFolderSelected))
                {
                    lastFolderSelected = (new KnownFolder(KnownFolderType.Downloads)).Path;
                }

                dialog.SelectedPath = lastFolderSelected;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK) return;

                TxtSourcePath.Text = dialog.SelectedPath;
                lastFolderSelected = dialog.SelectedPath;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtSourcePath.Text = "";
            TxtSourcePath.Focus();
        }

        private void OpenSettingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenSettingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new ExtensionSetting()
            {
                Owner = this,
            };

            dialog.ShowDialog();
        }
    }

    public enum State
    {
        Normal,
        Processing,
        Dropping
    }
}
