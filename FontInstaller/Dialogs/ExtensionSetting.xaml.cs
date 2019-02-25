using FontInstaller.Core;
using System.Collections.ObjectModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FontInstaller.Dialogs
{
    /// <summary>
    /// Interaction logic for ExtensionSetting.xaml
    /// </summary>
    public partial class ExtensionSetting : Window
    {
        public ObservableCollection<FontExtension> FontExtensions { get; set; }
        public ExtensionSetting()
        {
            InitializeComponent();
            FontExtensions = FontHelper.Instance.GetExtSettingList();
            ListViewExtensions.ItemsSource = FontExtensions;
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            FontHelper.Instance.UpdateDoc(FontExtensions);
            DialogResult = true;
        }

        private void CloseCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AddExtCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NewExt != null && !string.IsNullOrEmpty(NewExt.Text);
        }
        private void AddExtCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var ext = NewExt.Text;
            if (!ext.StartsWith("."))
            {
                ext = "." + ext;
            }

            if (ExtensionExists(ext))
            {
                SystemSounds.Beep.Play();
                return;
            }

            FontExtensions.Add(new FontExtension()
            {
                Ext = ext,
                IsVisible = true,
                IsRemovable = true
            });

            NewExt.Text = "";
            NewExt.Focus();
        }

        private bool ExtensionExists(string newExt)
        {
            foreach(var ext in FontExtensions)
            {
                if (ext.Ext == newExt)
                {
                    return true;
                }
            }

            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;

            if (!(button.DataContext is FontExtension fontExt)) return;

            FontExtensions.Remove(fontExt);
        }
    }
}
