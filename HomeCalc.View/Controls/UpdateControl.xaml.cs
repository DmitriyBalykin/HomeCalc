using HomeCalc.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HomeCalc.View.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    ///
    public partial class UpdateControl : UserControl
    {
        public UpdateControl()
        {
            InitializeComponent();

            var updateViewModel = new UpdateViewModel();

            updateViewModel.CloseApplicationEventHandler += (o, e) => Application.Current.Shutdown();

            this.DataContext = updateViewModel;
        }
    }
}
