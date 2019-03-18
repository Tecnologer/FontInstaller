using FontInstaller.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FontInstaller.Core
{
    public class CompressedFile
    {
        public string FilePath { get; private set; }
        public string DestinationPath { get; private set; }
        public string Password { get; set; }
        public bool IsPasswordSetted { get; set; }
        public bool CancelDecryption { get; set; }

        public readonly BackgroundWorker Worker;
        public readonly DoWorkEventArgs WorkerArgs;

        public CompressedFile(string filePath, string destPath, BackgroundWorker worker, DoWorkEventArgs workerArgs)
        {
            FilePath = filePath;
            DestinationPath = destPath;
            Worker = worker;
            WorkerArgs = workerArgs;
        }

        public void RequestPassword()
        {
            var progress = new ReportProgress() { State = InstalationState.PasswordRequired };
            Worker.ReportProgress(0, progress);

            while (!IsPasswordSetted && !CancelDecryption)
            {
                if (Worker.CancellationPending)
                {
                    return;
                }

                Thread.Sleep(100);
            }
        }

        public void WaitForPassword()
        {
            while (!IsPasswordSetted)
            {
                Thread.Sleep(100);
            }
        }
    }

    public class CompressedState
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public CompressedStates State { get; set; }
    }

    public enum CompressedStates
    {
        None,
        HasPassword,
        Error
    }
}
