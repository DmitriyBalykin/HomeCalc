using HomeCalc.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HomeCalc.View.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    ///
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((dynamic)DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
            }
        }
    }
}
