namespace FontInstaller.Core
{
    public class CSevenZip
    {
    //    public CSevenZip()
    //    {            
    //        SevenZipBase.SetLibraryPath(Constants.SEVENZIP_DLL_PATH);
    //    }

    //    public string ZipPassword { get; set; }
    //    public bool IsPasswordSetted { get; set; }
    //    public bool CancelDecryption { get; set; }

    //    private BackgroundWorker worker;

    //    public WorkerResult ExtractArchive(string zipPath, string outputFolder, BackgroundWorker worker)
    //    {
    //        this.worker = worker;
    //        //ReadOnlyCollection<string> readOnlyArchiveFilenames;
    //        //ReadOnlyCollection<string> readOnlyVolumeFilenames;
            
    //        var (extr, r) = GetExtractor(zipPath);

    //        if(r != null)
    //        {
    //            return r;
    //        }

    //        try
    //        {
    //            //readOnlyArchiveFilenames = extr.ArchiveFileNames;
    //            //readOnlyVolumeFilenames = extr.VolumeFileNames;
    //            extr.Extracting += Extr_Extracting;
    //            //extr.FileExtractionStarted += extr_FileExtractionStarted;
    //            ////extr.FileExists += extr_FileExists;
    //            //extr.ExtractionFinished += extr_ExtractionFinished;

    //            extr.ExtractArchive(outputFolder);
    //        }
    //        catch (FileNotFoundException error)
    //        {
    //            return new WorkerResult(false, error.Message);
    //        }
    //        finally
    //        {
    //            if (extr != null)
    //            {
    //                extr.Dispose();
    //            }
    //        }

    //        return new WorkerResult(false, null);
    //    }

    //    private (SevenZipExtractor extr, WorkerResult r) GetExtractor(string zipPath)
    //    {
    //        SevenZipExtractor extr = null;
    //        ZipPassword = null;
    //        IsPasswordSetted = false;
    //        var hasPassword = HasPassword(zipPath);

    //        if (hasPassword)
    //        {
    //            ValidatePassword(zipPath);
    //            if (worker.CancellationPending || CancelDecryption)
    //            {
    //                return (extr, new WorkerResult(true, "Extraccion cancelada por el usuario"));
    //            }
    //        }
            
    //        if (hasPassword)
    //        {
    //            extr = new SevenZipExtractor(zipPath, ZipPassword);
    //        }
    //        else
    //        {
    //            extr = new SevenZipExtractor(zipPath);
    //        }

    //        return (extr, null);
    //    }
        
    //    private void ValidatePassword(string zipPath)
    //    {
    //        var success = false;

    //        while (!success && !CancelDecryption)
    //        {
    //            RequestPassword();
    //            if (worker.CancellationPending)
    //            {
    //                return;
    //            }

    //            try
    //            {
    //                var fileType = Compressed.GetZipType(zipPath);
    //                using (SevenZipExtractor extr = new SevenZipExtractor(zipPath, ZipPassword, fileType))
    //                {
    //                    System.Diagnostics.Debug.WriteLine(extr.Format);
    //                    System.Diagnostics.Debug.WriteLine(extr.Password);
    //                    success = extr.Check();
    //                }
    //            }
    //            catch(Exception e)
    //            {
    //                System.Diagnostics.Debug.WriteLine(e);
    //                IsPasswordSetted = false;
    //                ZipPassword = null;
    //            }
    //        }
    //    }

    //    private bool HasPassword(string zipPath)
    //    {
    //        try
    //        {
    //            using (Stream stream = File.OpenRead(zipPath))
    //            {
    //                using (SevenZipExtractor extr = new SevenZipExtractor(stream))
    //                {
    //                    return extr.Check();
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            return e.Message.Contains("wrong password");
    //        }
    //    }

    //    private void RequestPassword()
    //    {
    //        var progress = new ReportProgress() { State = InstalationState.PasswordRequired };
    //        worker.ReportProgress(0, progress);

    //        while (!IsPasswordSetted && !CancelDecryption)
    //        {
    //            if (worker.CancellationPending)
    //            {
    //                return;
    //            }

    //            Thread.Sleep(100);
    //        }

    //    }

    //    private void Extr_Extracting(object sender, ProgressEventArgs e)
    //    {
    //        if (worker.CancellationPending)
    //        {
    //            //e. = true;
    //            return;
    //        }
    //        worker.ReportProgress(0, new ReportProgress() { Current = e.PercentDone, Total = 100 });
    //    }
    }
}
