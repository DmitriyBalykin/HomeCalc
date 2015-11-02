﻿using HomeCalc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeCalc.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        StatusService statusService;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            statusService = StatusService.GetInstance();
            statusService.StatusChanged += statusService_StatusChanged;
            statusService.ProgressUpdated += statusService_ProgressUpdated;

            Status = statusService.GetStatus();
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
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
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
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProgressValue"));
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
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowProgress"));
                }
            }
        }

        
    }
}
