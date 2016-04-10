using HomeCalc.Core.Helpers;
using HomeCalc.Presentation.ViewModels;
using System;
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
        public event EventHandler CloseApplicationEventHandler;
        public UpdateControl()
        {
            InitializeComponent();

            var updateViewModel = new UpdateViewModel();

            updateViewModel.CloseApplicationEventHandler += updateViewModel_CloseApplicationEventHandler;

            this.DataContext = updateViewModel;
        }

        void updateViewModel_CloseApplicationEventHandler(object sender, System.EventArgs e)
        {
            UIDispatcherHelper.CallOnUIThread(() => Application.Current.Shutdown());
        }
    }
}
