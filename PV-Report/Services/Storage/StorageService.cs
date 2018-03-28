using MimeKit;
using PvReport.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PvReport.Services.Storage
{
    public static class StorageService
    {
        private const string ConfigRepository = "Config";
        public const string PvReportsRepository = "PvReports";
        private const string PvReportMessagesRepository = "PvReportMails";
        private const string SyncronizationInfoFileName = "SyncInfo.cfg";

        public static SyncSettingsModel LoadSynchronizationInfo()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigRepository, SyncronizationInfoFileName);

            if (File.Exists(path))
            {
                var syncInfoJson = File.ReadAllText(path);
                var deserialized = SerializationService.JsonDeserialize<SyncSettingsModel>(syncInfoJson);
                if (deserialized != default(SyncSettingsModel))
                    return deserialized;
            }

            return SyncSettingsModel.Default;
        }

        public static bool SaveSynchronizationInfo(SyncSettingsModel info)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), ConfigRepository);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var infoJson = SerializationService.JsonSerialize(info);
            using (var fs = new FileStream(Path.Combine(directory, SyncronizationInfoFileName), FileMode.Create))
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), PvReportMessagesRepository);
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path))
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
            var directory = Path.Combine(Directory.GetCurrentDirectory(), PvReportMessagesRepository);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var mimeMsgArray = mimeMessages as MimeMessage[] ?? mimeMessages.ToArray();
            for (var i = 0; i < mimeMsgArray.Count(); i++)
            {
                var mimeMessage = mimeMsgArray[i];
                {
                    using (var fs = new FileStream(Path.Combine(directory, $"MimeMessage_{mimeMessage.Date.DateTime:dd.MM.yyyy}-{mimeMessage.Date.DateTime:HH.mm}.eml"), FileMode.Create))
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
            var directory = Path.Combine(Directory.GetCurrentDirectory(), PvReportsRepository, subDirectory);

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
            var directory = Path.Combine(Directory.GetCurrentDirectory(), PvReportsRepository, subDirectory);
            
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
