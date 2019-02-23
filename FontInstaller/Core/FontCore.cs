using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.ComponentModel;

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

        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff")</param>
        private static void RegisterFont(string fontSource)
        {
            var contentFontName = Path.GetFileName(fontSource);
 

            // Retrieves font name
            // Makes sure you reference System.Drawing
            PrivateFontCollection fontCol = new PrivateFontCollection();
            fontCol.AddFontFile(fontSource);
            var actualFontName = fontCol.Families[0].Name;

            //Add font
            AddFontResource(fontSource);
            //Add registry entry   
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",actualFontName, contentFontName, RegistryValueKind.String);

        }

        public void InstallFonts(string source, BackgroundWorker worker)
        {
            var progress = new ReportProgress();
            worker.ReportProgress(0, progress);

            var extensions = FontHelper.Instance.GetExtensionsFonts().ToList();
            string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)
                                .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            int current = 0;
            progress.Total = files.Length;
            foreach(var file in files)
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
        Installing
    }
}
