using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using HomeCalc.Presentation.Services;
using System.IO;
using System.Net;
using System.Net.Mime;
using HomeCalc.Core.Services;

namespace HomeCalc.Presentation.ViewModels
{
    public class HelpControlViewModel : ViewModel
    {
        public HelpControlViewModel()
        {
            AddCommand("SendLogCommand", new DelegateCommand(SendLogCommandExecute));
        }

        public bool SendEmailAuto
        {
            get
            { return Settings.GetSetting(SettingsService.SEND_EMAIL_AUTO_KEY).SettingBoolValue; }

            set
            {
                if (value != Settings.GetSetting(SettingsService.SEND_EMAIL_AUTO_KEY).SettingBoolValue)
                {
                    OnPropertyChanged(() => SendEmailAuto, value);
                }
            }
        }

        private void SendLogCommandExecute(object obj)
        {
            EmailSender.SendLogFile();
        }
    }
}
