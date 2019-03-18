using System.Text.RegularExpressions;

namespace FontInstaller.Core
{
    public class Compressed
    {
        private static readonly Regex reIsZip = new Regex(@".*\.zip$");
        private static readonly Regex reIsRar = new Regex(@".*\.rar$");
        private static readonly Regex reIsTar = new Regex(@".*\.tar$");
        private static readonly Regex reIs7z = new Regex(@".*\.7z$");

        public static bool IsCompressedFile(string filePath)
        {
            return reIsZip.IsMatch(filePath) ||
                reIsRar.IsMatch(filePath) ||
                reIsTar.IsMatch(filePath) ||
                reIs7z.IsMatch(filePath);
        }

        public static void UnZip(string filePath)
        {
            var fileType = GetZipType(filePath);
            switch (fileType)
            {
                case InArchiveFormat.Rar:
                    break;
                case InArchiveFormat.Tar:
                    break;
                case InArchiveFormat.SevenZip:
                    break;
                case InArchiveFormat.Zip:
                default:
                    break;
            }
        }

        public static InArchiveFormat GetZipType(string filePath)
        {
            if (reIsZip.IsMatch(filePath))
            {
                return InArchiveFormat.Zip;
            }

            if (reIsRar.IsMatch(filePath))
            {
                return InArchiveFormat.Rar;
            }

            if (reIsTar.IsMatch(filePath))
            {
                return InArchiveFormat.Tar;
            }

            return InArchiveFormat.SevenZip;
        }
    }

    public enum InArchiveFormat
    {
        Zip,
        Rar,
        Tar,
        SevenZip
    }
}
