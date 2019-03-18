using FontInstaller.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace FontInstaller.Core.Helpers
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
            InitializeSettings();
        }

        public IEnumerable<string> GetExtensionsFonts()
        {
            try
            {
                var doc = XDocument.Load(Constants.SETTING_XML_PATH);
                List<string> exts = new List<string>();
                var xmlFonts = doc.Element(Constants.XML_FONTS);
                foreach (XElement e in xmlFonts.Elements(Constants.XML_FONT))
                {
                    var ext = e.Attribute(Constants.XML_ATTR_EXT).Value;

                    var visibility = Convert.ToBoolean(e.Attribute(Constants.XML_ATTR_VIS).Value);
                    if (!visibility) continue;

                    if (!ext.StartsWith("."))
                    {
                        ext = "." + ext;
                    }
                    exts.Add(ext);
                }

                return exts;
            }
            catch (Exception e)
            {
                Logger.Log("Error when try retrieve list of supported extensions", e);
                return null;
            }
        }

        public ObservableCollection<FontExtension> GetExtSettingList()
        {
            try
            {
                var doc = XDocument.Load(Constants.SETTING_XML_PATH);
                var list = new ObservableCollection<FontExtension>();
                var xmlFonts = doc.Element(Constants.XML_FONTS);
                foreach (XElement e in xmlFonts.Elements(Constants.XML_FONT))
                {
                    var ext = e.Attribute(Constants.XML_ATTR_EXT).Value;
                    var visibility = Convert.ToBoolean(e.Attribute(Constants.XML_ATTR_VIS).Value);
                    var removable = Convert.ToBoolean(e.Attribute(Constants.XML_ATTR_REM).Value);

                    if (!ext.StartsWith("."))
                    {
                        ext = "." + ext;
                    }
                    list.Add(new FontExtension()
                    {
                        Ext = ext,
                        IsVisible = visibility,
                        IsRemovable = removable
                    });
                }
                return list;
            }
            catch (Exception e)
            {
                Logger.Log("Error when try retrieve settings", e);
                return null;
            }
        }

        public bool UpdateDoc(IEnumerable<FontExtension> fontExtensions)
        {
            try
            {
                var doc = XDocument.Load(Constants.SETTING_XML_PATH);

                doc.RemoveNodes();
                XElement xmlFonts = new XElement(Constants.XML_FONTS);
                foreach(var fontExt in fontExtensions)
                {
                    var singleFont = new XElement(Constants.XML_FONT, 
                        new XAttribute(Constants.XML_ATTR_EXT, fontExt.Ext),
                        new XAttribute(Constants.XML_ATTR_VIS, fontExt.IsVisible),
                        new XAttribute(Constants.XML_ATTR_REM, fontExt.IsRemovable)
                        );

                    xmlFonts.Add(singleFont);
                }

                doc.Add(xmlFonts);
                doc.Save(Constants.SETTING_XML_PATH);
                return true;
            }
            catch(Exception e)
            {
                Logger.Log("Error when try save settings", e);
                return false;
            }
        }

        private XDocument LoadDefaultDocument()
        {
            var uri = new Uri("/Resources/FontExtensions.xml", UriKind.RelativeOrAbsolute);
            using (var st = Application.GetResourceStream(uri).Stream)
            {

                return XDocument.Load(st);
            }
        }

        private void InitializeSettings()
        {
            if (!Directory.Exists(Constants.SETTING_FOLDER_PATH))
            {
                Directory.CreateDirectory(Constants.SETTING_FOLDER_PATH);
            }

            if (!File.Exists(Constants.SETTING_XML_PATH))
            {
                var doc = LoadDefaultDocument();
                doc.Save(Constants.SETTING_XML_PATH);
            }
        }
    }

    public class FontExtension
    {
        public string Ext {get; set; }
        public bool IsVisible { get; set; }
        public bool IsRemovable { get; set; }
    }
}
