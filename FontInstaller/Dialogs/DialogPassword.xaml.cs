using System.Windows;

namespace FontInstaller.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogPassword.xaml
    /// </summary>
    public partial class DialogPassword : Window
    {
        public DialogPassword()
        {
            InitializeComponent();
        }

        private void AcceptCommand_CanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AcceptCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CloseCommand_CanExecuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
