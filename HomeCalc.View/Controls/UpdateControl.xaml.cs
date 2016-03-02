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
            this.DataContext = new UpdateViewModel();
        }
    }
}
