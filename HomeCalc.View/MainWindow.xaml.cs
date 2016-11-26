using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
using HomeCalc.Core.Services.Messages;
using System;
using System.ComponentModel;
using System.Threading;
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

        private MessageDispatcher MsgDispatcher;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            MsgDispatcher = MessageDispatcher.GetInstance();

            MsgDispatcher.AddHandler(MessageHandler);

            statusService = StatusService.GetInstance();
            updateService = UpdateService.GetInstance();
            logger = LogService.GetLogger();

            Status = statusService.GetStatus();
        }

        private void MessageHandler(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.STATUS_CHANGED:
                    Status = message.Data as string;
                    break;
                case MessageType.PROGRESS_UPDATED:
                    ProgressValue = (int)message.Data;
                    break;
                case MessageType.PROGRESS_STARTED:
                    ShowProgress = true;
                    break;
                case MessageType.PROGRESS_FINISHED:
                    ShowProgress = false;
                    break;
                case MessageType.UPDATES_AVAILABLE:
                    UpdateColorNotify = true;
                    break;

                default:
                    break;
            }
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
