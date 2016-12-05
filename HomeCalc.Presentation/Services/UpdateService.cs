using HomeCalc.Core.Helpers;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Services.Messages;
using HomeCalc.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace HomeCalc.Core.Services
{
    public class UpdateService
    {
        private static UpdateService instance;
        private Logger logger;
        private MessageDispatcher MsgDispatcher;
        private UpdateService()
        {
            MsgDispatcher = MessageDispatcher.GetInstance();

            logger = LogService.LogService.GetLogger();
            logger.SendEmail = SettingsService.GetInstance().GetSetting(SettingsService.SEND_EMAIL_AUTO_KEY).SettingBoolValue;
        }
        public static UpdateService GetInstance()
        {
            if (instance == null)
            {
                instance = new UpdateService();
            }
            return instance;
        }

        public async Task<string> GetUpdatesInformation()
        {
            string result = "";
            try
            {
                var updatesInfo = await VersionChecker.GetUpdatesInformation(true);

                if (!updatesInfo.HasNewVersion)
                {
                    result = "Версія програми є найновішою.";
                }
                else
                {
                    var sb = new StringBuilder();

                    foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                    {
                        sb.AppendLine(updateVersion.ToString());
                        sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                        sb.AppendLine();
                    }
                    result = sb.ToString();

                    MsgDispatcher.Post(MessageType.UPDATES_AVAILABLE);
                }
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }
            return result;
        }

        public async Task<string> GetUpdatesHistory()
        {
            string result = "";
            try
            {
                var updatesInfo = await VersionChecker.GetUpdatesInformation(false);
                if (updatesInfo.ChangesByVersions.Keys.Count() == 0)
                {
                    result = "Історія оновлень не знайдена.";
                }
                else
                {
                    var sb = new StringBuilder();

                    foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
                    {
                        sb.AppendLine(updateVersion.ToString());
                        sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                        sb.AppendLine();
                    }
                    result = sb.ToString();
                }
            }
            catch (WebException ex)
            {
                logger.Error("Error occured during update download");
                logger.Error(ex.Message);
            }
            return result;
        }
    }
}
