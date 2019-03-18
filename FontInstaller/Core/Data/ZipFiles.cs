using FontInstaller.Core.Helpers;
using Ionic.Zip;
using System;
using System.ComponentModel;

namespace FontInstaller.Core.Data
{
    public class ZipFile: CompressedFile
    {
        public ZipFile(string filePath, string destPath, BackgroundWorker worker, DoWorkEventArgs workerArgs) : 
            base(filePath, destPath, worker, workerArgs)
        {
            
        }
        public CompressedState ExtractAll()
        {
            var result = HasPassword();
            if (result.State == CompressedStates.HasPassword)
            {
                RequestPassword();
                result = ExtractFonts();
            }

            return result;
        }
        private CompressedState HasPassword()
        {
            var result = new CompressedState();
            try
            {
                using (Ionic.Zip.ZipFile archive = new Ionic.Zip.ZipFile(FilePath))
                {
                    archive.ExtractAll(DestinationPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch(BadPasswordException)
            {
                result.State = CompressedStates.HasPassword;
            }
            catch(Exception e)
            {
                result.State = CompressedStates.Error;
                result.Message = "fallo durante la comprobacion de contraseña";
                Logger.Log("Check password failed", e);
                
            }

            return result;
        }

        private CompressedState ExtractFonts()
        {
            var result = new CompressedState();
            try
            {
                using (Ionic.Zip.ZipFile archive = new Ionic.Zip.ZipFile(FilePath))
                {
                    archive.Password = Password;
                    archive.Encryption = EncryptionAlgorithm.PkzipWeak;
                    archive.ExtractAll(DestinationPath, ExtractExistingFileAction.OverwriteSilently);

                    return result;
                }
            }
            catch (Exception e)
            {
                Logger.Log("Extraction error", e);

                result.State = CompressedStates.Error;
                result.Message = "Error al extraer la fuentes, favor de revisar el archivo antes de continuar";
                return result;
            }
        }
    }
}
