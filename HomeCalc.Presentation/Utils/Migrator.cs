using HomeCalc.Core.LogService;
using HomeCalc.Core.Utilities;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Utils
{
    public class Migrator
    {
        private static Logger logger = LogService.GetLogger();
        public static async Task<MigrationResult> MigrateFromCsv(string folderPath, Action<MigrationResultArgs> DataMigrationStatusUpdated)
        {
            string filePath = Path.Combine(folderPath, FileUtilities.HOMEEX_DATA_FILE);
            if (!FileUtilities.FileExists(filePath))
            {
                logger.Warn("File with data absent at path: {0}", filePath);
                return null;
            }

            var ctsource = new CancellationTokenSource();
            var progress = new Progress<MigrationResultArgs>(DataMigrationStatusUpdated);
            //TODO implement cancel
            return await StartMigrationTask(filePath, progress, ctsource.Token);
        }

        private static async Task<MigrationResult> StartMigrationTask(string sourceFilePath, IProgress<MigrationResultArgs> progress, CancellationToken ctoken)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return null;
            }

            var content = File.ReadAllLines(sourceFilePath);

            int totalCount = content.Length;
            MigrationResult migrationResult = null;
            try
            {
                migrationResult = await Task.Run<MigrationResult>(() =>
                {
                    var taskResult = new MigrationResult();
                    var storageService = StorageService.GetInstance();
                    var purchaseList = new List<Purchase>(totalCount);

                    foreach (var line in File.ReadAllLines(sourceFilePath).Skip(1))
                    {
                        if (ctoken.IsCancellationRequested)
                        {
                            break;
                        }
                        try
                        {
                            var columns = line.Split(';');

                            purchaseList.Add(
                                new Purchase
                                {
                                    Date = DateTime.ParseExact(columns[1], "yyyyMMdd", CultureInfo.InvariantCulture),
                                    ItemCost = double.Parse(columns[5].Replace('.', ',')),
                                    ItemsNumber = double.Parse(columns[4].Replace('.', ',')),
                                    TotalCost = double.Parse(columns[6].Replace('.', ',')),
                                    Name = columns[3],
                                    Type = storageService.ResolvePurchaseType(name: columns[2])
                                });
                            taskResult.AddSucceededLine();
                        }
                        catch (Exception)
                        {
                            taskResult.AddFailedLine();
                        }
                        progress.Report(new MigrationResultArgs { Total = totalCount, Processed = taskResult.ResultProcessed });
                    }
                    storageService.SavePurchaseBulk(purchaseList);
                    return taskResult;
                });
            }
            catch (TaskCanceledException)
            { }

            return migrationResult;
        }
    }
    public class MigrationResult
    {
        public int ResultProcessed
        {
            get
            {
                return SucceededLines + FailedLines;
            }
            set
            { }
        }
        public int TotalLinesExpected { get; set; }
        public int SucceededLines { get; set; }
        public int FailedLines { get; set; }
        public List<int> FailedLinesNumbers { get; set; }
        public MigrationResult()
        {
            FailedLinesNumbers = new List<int>();
        }
        internal void AddFailedLine()
        {
            FailedLinesNumbers.Add(SucceededLines + FailedLines);
            FailedLines++;
        }

        internal void AddSucceededLine()
        {
            SucceededLines++;
        }
    }
    public class MigrationResultArgs : EventArgs
    {
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Percent
        {
            get
            {
                return 100 * Processed / Total;
            }
        }
    }
}
