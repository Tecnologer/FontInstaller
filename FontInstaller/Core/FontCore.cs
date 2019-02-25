using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace FontInstaller.Core
{
    public class FontCore
    {
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int CreateScalableFontResource(uint fdwHidden, string
        lpszFontRes, string lpszFontFile, string lpszCurrentPath);

        public bool IsPasswordSetted
        {
            set
            {
                sevenZip.IsPasswordSetted = value;
            }
        }
        public string ZipPassword
        {
            set
            {
                sevenZip.ZipPassword = value;
            }
        }

        private CSevenZip sevenZip = new CSevenZip();

        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff")</param>
        private void RegisterFont(string fontSource)
        {
            var contentFontName = Path.GetFileName(fontSource);
            var pathDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), contentFontName);

            if (File.Exists(pathDest))
            {
                return;
            }
            // Retrieves font name
            // Makes sure you reference System.Drawing
            File.Copy(fontSource, pathDest);
            PrivateFontCollection fontCol = new PrivateFontCollection();
            fontCol.AddFontFile(pathDest);
            var actualFontName = fontCol.Families[0].Name;

            //Add font
            AddFontResource(pathDest);
            //Add registry entry   
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",actualFontName, contentFontName, RegistryValueKind.String);

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

                RegisterFont(file);

                progress.Current = current++;
                worker.ReportProgress(0, progress);
            }
        }

        public void InstallFontsFromZip(string path, BackgroundWorker worker, DoWorkEventArgs workerEvents)
        {
            string outFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path));
            try
            {
                //var extractState = ExtractZipFile(path, outFolder, worker);
                var extractState = sevenZip.ExtractArchive(path, outFolder, worker);
                if (extractState.HasError)
                {
                    workerEvents.Result = extractState;
                    return;
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
