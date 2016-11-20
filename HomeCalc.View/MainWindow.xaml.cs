using HomeCalc.Core.LogService;
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
        private const int WindowHeightDefaultValue = 550;

        private StatusService statusService;
        private UpdateService updateService;
        private Logger logger;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            statusService = StatusService.GetInstance();
            updateService = UpdateService.GetInstance();
            logger = LogService.GetLogger();

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
            ProgressValue = e.Progress;
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

        private double progressValue;
        public double ProgressValue
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

        public double WindowMaxHeight
        {
            get
            {
                logger.Debug("Current max window max height: {0}", SystemParameters.PrimaryScreenHeight);

                return SystemParameters.PrimaryScreenHeight;
            }
        }

        public double WindowHeight
        {
            set
            {
                if ((WindowTop + value) > SystemParameters.PrimaryScreenHeight)
                {
                    logger.Debug("Set window top: {0}", (SystemParameters.PrimaryScreenHeight - value));

                    WindowTop = SystemParameters.PrimaryScreenHeight - value;
                }
            }
        }

        private double windowTop;
        public double WindowTop
        {
            get
            {
                return windowTop;
            }
            set
            {
                logger.Debug("New window top: {0}", value);

                windowTop = value;
                OnUpdateProperty("WindowTop");
            }
        }

        private void OnUpdateProperty(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
