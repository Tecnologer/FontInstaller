using System;
using System.IO;
using System.Reflection;

namespace FontInstaller.Resources
{
    public static class Constants
    {
        public const string FOLDER_NAME = "Tecnologer.FontInstaller";
        public const string XML_NAME = "Settings.xml";

        // XML Elements
        public const string XML_FONTS = "Fonts";
        public const string XML_FONT = "Font";
        public const string XML_ATTR_EXT = "Ext";
        public const string XML_ATTR_VIS = "IsVisible";
        public const string XML_ATTR_REM = "IsRemovable";

        public static readonly string APPDATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string SETTING_FOLDER_PATH = Path.Combine(APPDATA, FOLDER_NAME);
        public static readonly string SETTING_XML_PATH = Path.Combine(SETTING_FOLDER_PATH, XML_NAME);

        public static readonly string SEVENZIP_DLL_PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
    }
}
