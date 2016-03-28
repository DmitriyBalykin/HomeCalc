using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Updater;

namespace HomeCalc.Presentation.ViewModels
{
    public class UpdateViewModel : ViewModel
    {
        public event EventHandler CloseApplicationEventHandler;
        public UpdateViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("CheckUpdates", new DelegateCommand(CheckUpdatesCommandExecute));
            AddCommand("Update", new DelegateCommand(UpdateCommandExecute));
        }

        private void CheckUpdatesCommandExecute(object obj)
        {
            var updatesInfo = VersionChecker.GetUpdatesInformation();

            if (!updatesInfo.HasNewVersion)
            {
                VersionChanges = "Версія програми є найновішою.";
                return;
            }

            var sb = new StringBuilder();

            foreach (var updateVersion in updatesInfo.ChangesByVersions.Keys)
            {
                sb.AppendLine(updateVersion.ToString());
                sb.AppendLine(updatesInfo.ChangesByVersions[updateVersion]);
                sb.AppendLine();
            }
            VersionChanges = sb.ToString();
        }

        private void UpdateCommandExecute(object obj)
        {
            if (CloseApplicationEventHandler != null)
	        {
		         CloseApplicationEventHandler(this, EventArgs.Empty);
	        }
            
            var updateStartResult = VersionUpdater.Update();

            if (string.IsNullOrEmpty(updateStartResult))
            {
                if (CloseApplicationEventHandler != null)
                {
                    CloseApplicationEventHandler(this, EventArgs.Empty);
                }
            }
        }

        private string versionChanges;
        public string VersionChanges
        {
            get { return versionChanges; }
            set
            {
                if (value != versionChanges)
                {
                    versionChanges = value;
                    OnPropertyChanged(() => VersionChanges);
                }
            }
        }
        
    }
}
