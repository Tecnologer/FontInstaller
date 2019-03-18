using FontInstaller.Core.Data;
using FontInstaller.Core.Helpers;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace FontInstaller.Core
{
    public class FontCore
    {
        public bool IsPasswordSetted
        {
            set
            {
                if (compressedFile == null) return;
                compressedFile.IsPasswordSetted = value;
            }
        }
        public bool CancelDecryption
        {
            set
            {
                if (compressedFile == null) return;
                compressedFile.CancelDecryption = value;
            }
        }
        public string ZipPassword
        {
            set
            {
                if (compressedFile == null) return;
                compressedFile.Password = value;
            }
        }

        private CompressedFile compressedFile;

        internal static void RegiterFont(string fontPath)
        {
            string fontName = Path.GetFileNameWithoutExtension(fontPath);
            string cmd = string.Format("copy /Y \"{0}\" \"%WINDIR%\\Fonts\" ", fontPath);
            ExecuteCommand(cmd);

            System.IO.FileInfo FInfo = new System.IO.FileInfo(fontPath);
            cmd = string.Format("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts\" /v \"{0}\" /t REG_SZ /d {1} /f", fontName, FInfo.Name);
            ExecuteCommand(cmd);
        }

        public static void ExecuteCommand(string command)
        {
            System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo("cmd.exe")
            {
                Arguments = string.Format("/c {0}", command),
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process.Start(Info);
        }

        public void InstallFontsFromFolder(string source, BackgroundWorker worker)
        {
            var progress = new ReportProgress();
            worker.ReportProgress(0, progress);

            var extensions = FontHelper.Instance.GetExtensionsFonts().ToList();
            string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)
                                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            int current = 0;
            progress.Total = files.Length;
            progress.State = InstalationState.Installing;
            foreach (var file in files)
            {
                if (worker.CancellationPending)
                {
                    return;
                }

                RegiterFont(file);

                progress.Current = current++;
                worker.ReportProgress(0, progress);
            }
        }

        public void InstallFontsFromZip(string path, BackgroundWorker worker, DoWorkEventArgs workerEvents)
        {
            string outFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path));
            try
            {
                switch (Compressed.GetZipType(path))
                {
                    case InArchiveFormat.Zip:
                        compressedFile = new ZipFile(path, outFolder, worker, workerEvents);
                        var result = (compressedFile as ZipFile).ExtractAll();
                        if (result.State == CompressedStates.Error)
                        {
                            workerEvents.Cancel = true;
                            return;
                        }
                        break;
                    case InArchiveFormat.Rar:
                        break;
                    case InArchiveFormat.Tar:
                        break;
                    case InArchiveFormat.SevenZip:
                        break;
                    default:
                        break;
                }

                InstallFontsFromFolder(outFolder, worker);
            }
            finally
            {
                Thread.Sleep(100);
                if (Directory.Exists(outFolder))
                {
                    Directory.Delete(outFolder, true);
                }
            }
        }
    }

    public class ReportProgress
    {
        public int Percentage { get; set; } 
        public int Current { get; set; }
        public int Total { get; set; }
        public InstalationState State { get; set; }
        public string Label { get; set; }
    }

    public enum InstalationState
    {
        OpenFile,
        SearchFont,
        Installing,
        PasswordRequired,
        Extracting
    }
    
    public class WorkerResult
    {
        public bool HasError { get; set; }
        public string Msg { get; set; }

        public WorkerResult(bool e, string m)
        {
            HasError = e;
            Msg = m;
        }
    }
}
