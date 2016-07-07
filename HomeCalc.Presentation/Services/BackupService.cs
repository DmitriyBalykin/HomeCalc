using HomeCalc.Core.LogService;
using HomeCalc.Core.Services;
using HomeCalc.Model.DbService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeCalc.Presentation.Services
{
    public class BackupService
    {

        public static void BackupDatabase()
        {
            var logger = LogService.GetLogger();
            var statusService = StatusService.GetInstance();
            var settingsService = SettingsService.GetInstance();
            var backupDirectory = settingsService.GetStringValue(SettingsService.BACKUP_PATH_KEY);

            if (settingsService.GetBooleanValue(SettingsService.DO_DATABASE_BACKUP) &&
                !string.IsNullOrWhiteSpace(backupDirectory))
            {
                var backupPath = Path.Combine(backupDirectory, FilenameService.DB_FILE_NAME);
                var backupFolder = Directory.GetParent(backupPath);
                if (!backupFolder.Exists)
                {
                    logger.Error("Backup destination folder missing.");
                    MessageBox.Show("Папка для резервної копії бази даних недоступна.");
                    return;
                }
                //backing up of backup
                var originPath = FilenameService.GetDBPath();
                var backup2Path = backupPath + ".bak";
                if (File.Exists(backupPath))
                {
                    try
                    {
                        File.Copy(backupPath, backup2Path, true);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Backup duplication failed.");
                        logger.Error(ex.Message);
                        MessageBox.Show("Помилка при резервуванні бази даних.");
                        return;
                    }
                }
                //main backup step
                try
                {
                    File.Copy(originPath, backupPath, true);
                }
                catch (Exception ex)
                {
                    logger.Error("Backup copying failed.");
                    logger.Error(ex.Message);
                    MessageBox.Show("Помилка при резервуванні бази даних.");
                    return;
                }
                //compare checksums of origin and backup
                string md5Origin = null;
                string md5Backup = null;
                using (var md5 = MD5.Create())
                {

                    using (var originStream = File.Open(originPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        md5Origin = BitConverter.ToString(md5.ComputeHash(originStream));
                    }
                    using (var backupStream = File.Open(backupPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        md5Backup = BitConverter.ToString(md5.ComputeHash(backupStream));
                    }
                }
                if (md5Origin == md5Backup)
                {
                    statusService.Post("Резервування бази даних виконано успішно");
                    if (File.Exists(backup2Path))
                    {
                        File.Delete(backup2Path);
                    }
                }
                else
                {
                    logger.Error("Backup copying failed. MD5 hashes of source and destination not equals.");
                    MessageBox.Show("Помилка при резервуванні бази даних.");
                    File.Delete(backupPath);
                    if (File.Exists(backup2Path))
                    {
                        File.Move(backup2Path, backupPath);
                    }
                }
            }
        }
    }
}