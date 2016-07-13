using HomeCalc.Core.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace HomeCalc.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        StatusService statusService;
        UpdateService updateService;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            statusService = StatusService.GetInstance();
            updateService = UpdateService.GetInstance();

            statusService.StatusChanged += statusService_StatusChanged;
            statusService.ProgressUpdated += statusService_ProgressUpdated;
            statusService.ProgressStarted += statusService_ProgressStarted;
            statusService.ProgressStopped += statusService_ProgressStopped;

            Status = statusService.GetStatus();

            updateService.UpdatesAvailableEvent += updateService_UpdatesAvailableEvent;
        }

        void updateService_UpdatesAvailableEvent(object sender, EventArgs e)
        {
            UpdateColorNotify = true;
        }

        void statusService_ProgressStopped(object sender, EventArgs e)
        {
            ShowProgress = false;
        }

        void statusService_ProgressStarted(object sender, EventArgs e)
        {
            ShowProgress = true;
        }

        void statusService_ProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            ProgressValue = e.Progress.ToString();
        }

        void statusService_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            Status = e.Status;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    OnUpdateProperty("Status");
                }
            }
        }

        private string progressValue;
        public string ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    OnUpdateProperty("ProgressValue");
                }
            }
        }

        private bool showProgress = true;
        public bool ShowProgress
        {
            get
            {
                return showProgress;
            }
            set
            {
                if (value != showProgress)
                {
                    showProgress = value;
                    OnUpdateProperty("ShowProgress");
                }
            }
        }

        private bool updateColorNotify = false;
        public bool UpdateColorNotify
        {
            get
            {
                return updateColorNotify;
            }
            set
            {
                if (value != updateColorNotify)
                {
                    updateColorNotify = value;
                    OnUpdateProperty("UpdateColorNotify");
                }
            }
        }

        private int windowHeight;
        public int WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                if (value != windowHeight)
                {
                    windowHeight = value;
                    OnUpdateProperty("WindowHeight");
                }
            }
        }

        private void OnUpdateProperty(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void WindowSizeCheck(object sender, EventArgs e)
        {
            var height = System.Windows.SystemParameters.PrimaryScreenHeight;

            if (WindowHeight > height)
            {
                WindowHeight = (int)height;
            }
        }
    }
}
