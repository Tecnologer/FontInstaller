using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                case ZipType.Rar:
                    break;
                case ZipType.Tar:
                    break;
                case ZipType.SevenZ:
                    break;
                case ZipType.Zip:
                default:
                    break;
            }
        }

        public static ZipType GetZipType(string filePath)
        {
            if (reIsZip.IsMatch(filePath))
            {
                return ZipType.Zip;
            }

            if (reIsRar.IsMatch(filePath))
            {
                return ZipType.Rar;
            }

            if (reIsTar.IsMatch(filePath))
            {
                return ZipType.Tar;
            }

            return ZipType.SevenZ;
        }
    }

    public enum ZipType
    {
        Zip,
        Rar,
        Tar,
        SevenZ
    }
}
