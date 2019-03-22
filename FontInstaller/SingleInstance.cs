using System.Threading;

namespace FontInstaller
{
    public class SingleInstance
    {
        private const string MutexName = "Tecnologer.FontInstaller";
        private static bool firstApplicationInstance;
        private static Mutex mutexApplication;

        public static bool IsApplicationFirstInstance()
        {
            // Allow for multiple runs but only try and get the mutex once
            if (mutexApplication == null)
            {
                mutexApplication = new Mutex(true, MutexName, out firstApplicationInstance);
            }

            return firstApplicationInstance;
        }

        public static void Release()
        {
            if (mutexApplication != null)
            {
                mutexApplication.Dispose();
            }
        }
    }
}
