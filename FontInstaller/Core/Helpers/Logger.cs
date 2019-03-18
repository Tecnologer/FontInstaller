using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontInstaller.Core.Helpers
{
    public static class Logger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Error(Exception e)
        {
            log.Error(e.Message);
            log.Error(e.StackTrace);
        }

        public static void Info(string txt)
        {
            log.Info(txt);
        }

        public static void Log(params object[] elements)
        {
            foreach(var e in elements)
            {
                if(e is Exception ex)
                {
                    Error(ex);
                    continue;
                }

                Info(e.ToString());
            }
        }
    }
}
