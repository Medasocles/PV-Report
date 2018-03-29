using MimeKit;
using PvReport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PvReport.Services.Storage
{
    public static class StorageService
    {
        // static folder names and paths
        public static string RepositoryRootFolder => Environment.SpecialFolder.LocalApplicationData.ToString();
        public static string ConfigRepositoryPath => Path.Combine(RepositoryRootFolder, ConfigRepositoryName);
        public static string PvReportRepositoryName => "PvReports";
        public static string PvReportRepositoryPath => Path.Combine(RepositoryRootFolder, PvReportRepositoryName);
        public static string PvReportMailsRepositoryName => "PvReportMails";
        public static string PvReportMailsRepositoryPath => Path.Combine(RepositoryRootFolder, PvReportMailsRepositoryName); public static string ConfigRepositoryName => "Config";
        
        // static file names and paths
        public static string SyncSettingsFileName = "SyncSettings.cfg";
        public static string SyncSettingsFilePath = Path.Combine(ConfigRepositoryPath, SyncSettingsFileName);


        public static SyncSettingsModel LoadSynchronizationInfo()
        {
            if (File.Exists(SyncSettingsFilePath))
            {
                var syncInfoJson = File.ReadAllText(SyncSettingsFilePath);
                var deserialized = SerializationService.JsonDeserialize<SyncSettingsModel>(syncInfoJson);
                if (deserialized != default(SyncSettingsModel))
                    return deserialized;
            }

            return SyncSettingsModel.Default;
        }

        public static bool SaveSynchronizationInfo(SyncSettingsModel info)
        {
            if (!Directory.Exists(ConfigRepositoryPath))
                Directory.CreateDirectory(ConfigRepositoryPath);

            var infoJson = SerializationService.JsonSerialize(info);
            using (var fs = new FileStream(SyncSettingsFilePath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(infoJson);
                }
            }
            return true;
        }

        public static IEnumerable<MimeMessage> LoadMimeMessages()
        {
            var mimeMessages = new List<MimeMessage>();
            if (Directory.Exists(PvReportMailsRepositoryPath))
            {
                foreach (var file in Directory.GetFiles(PvReportMailsRepositoryPath))
                {
                    using (var fs = new FileStream(file, FileMode.Open))
                    {
                        var mimeMsgTask = MimeMessage.LoadAsync(ParserOptions.Default, fs);
                        mimeMessages.Add(mimeMsgTask.Result);
                    }
                }
            }

            return mimeMessages;
        }

        public static bool SaveMimeMessages(IEnumerable<MimeMessage> mimeMessages)
        {
            if (!Directory.Exists(PvReportMailsRepositoryPath))
                Directory.CreateDirectory(PvReportMailsRepositoryPath);

            var mimeMsgArray = mimeMessages as MimeMessage[] ?? mimeMessages.ToArray();
            for (var i = 0; i < mimeMsgArray.Count(); i++)
            {
                var mimeMessage = mimeMsgArray[i];
                {
                    using (var fs = new FileStream(Path.Combine(PvReportMailsRepositoryPath, $"MimeMessage_{mimeMessage.Date.DateTime:dd.MM.yyyy}-{mimeMessage.Date.DateTime:HH.mm}.eml"), FileMode.Create))
                    {
                        mimeMessage.WriteTo(FormatOptions.Default, fs);
                    }
                }
            }
            return true;
        }

        public static void SavePvReport(string reportFileName, IMimeContent reportMimeContent)
        {
            var subDirectory = CheckPvReportType(reportFileName);
            var directory = Path.Combine(PvReportRepositoryPath, subDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var fs = new FileStream(Path.Combine(directory, reportFileName), FileMode.Create))
            {
                reportMimeContent.DecodeTo(fs);
            }
        }

        public static void SavePvReport(string reportFileName, Stream stream)
        {
            var subDirectory = CheckPvReportType(reportFileName);
            var directory = Path.Combine(PvReportRepositoryPath, subDirectory);
            
            var streamBuffer = new byte[1024 * 4];
            using (var saveFileStream = new FileStream(Path.Combine(directory, reportFileName), FileMode.Create))
            {
                int receivedBytes;
                while ((receivedBytes = stream.Read(streamBuffer, 0, streamBuffer.Length)) > 0)
                {
                    saveFileStream.Write(streamBuffer, 0, receivedBytes);
                    saveFileStream.Flush();
                }
            }
        }

        private static string CheckPvReportType(string reportFileName)
        {
            var subDirectory = string.Empty;
            if (reportFileName.Contains("Produktion") || reportFileName.Contains("Daily-KWH-Report"))
                subDirectory = "Produktion";
            else if (reportFileName.Contains("Energiebilanz") || reportFileName.Contains("Daily-Energiebilanz-Report"))
                subDirectory = "Energiebilanz";
            return subDirectory;
        }

    }
}
