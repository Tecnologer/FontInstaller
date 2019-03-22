using System.Windows;

namespace FontInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!SingleInstance.IsApplicationFirstInstance())
            {
                MessageBox.Show("Solo puede ejecutarse una instancia a la vez.", "Ya esta en ejecucion", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SingleInstance.Release();
            base.OnExit(e);
        }
    }
}
