using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace FontInstaller.Core
{
    public class FontHelper
    {
        #region Singleton
        private static readonly object _syncRoot = new object();
        private static FontHelper instance;

        public static FontHelper Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    return instance ?? (instance = new FontHelper());
                }
            }
        }
        #endregion

        public FontHelper()
        {
            Uri uri = new Uri("/Resources/FontExtensions.XML", UriKind.RelativeOrAbsolute);
            System.IO.Stream st = Application.GetResourceStream(uri).Stream;
            doc.Load(st);
        }

        readonly XmlDocument doc = new XmlDocument();

        public IEnumerable<string> GetExtensionsFonts()
        {
            List<string> exts = new List<string>();
            var xmlFonts = doc.DocumentElement;
            foreach(XmlNode e in xmlFonts.SelectNodes("Font"))
            {
                var ext = e.Attributes["ext"].Value;
                if (!ext.StartsWith("."))
                {
                    ext = "." + ext;
                }
                exts.Add(ext);
            }

            return exts;
        }
    }
}
