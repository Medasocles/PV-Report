using MimeKit;
using PvReport.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PvReport.Services
{
    public static class RepositoryService
    {
        private const string ConfigRepository = "Config";
        private const string PvReportsRepository = "PvReports";
        private const string PvReportMessagesRepository = "PvReportMails";
        private const string SyncronizationInfoFileName = "SyncInfo.cfg";

        public static SynchronizationInfo LoadSynchronizationInfo()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigRepository, SyncronizationInfoFileName);

            if (File.Exists(path))
            {
                var syncInfoJson = File.ReadAllText(path);
                var deserialized = SerializationService.JsonDeserialize<SynchronizationInfo>(syncInfoJson);
                if (deserialized != default(SynchronizationInfo))
                    return deserialized;
            }

            return SynchronizationInfo.Empty;
        }

        public static bool SaveSynchronizationInfo(SynchronizationInfo info)
        {
            // todo: serialize and save

            return true;
        }

        public static IEnumerable<MimeMessage> LoadMailsFromStorage()
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

        public static bool SaveMailsToFile(IEnumerable<MimeMessage> mimeMessages)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), PvReportMessagesRepository);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var mimeMsgArray = mimeMessages as MimeMessage[] ?? mimeMessages.ToArray();
            for (var i = 0; i < mimeMsgArray.Count(); i++)
            {
                var mimeMessage = mimeMsgArray[i];
                {
                    using (var fs = new FileStream(Path.Combine(directory, $"MimeMessage_{i}.eml"), FileMode.Create))
                    {
                        mimeMessage.WriteTo(FormatOptions.Default, fs);
                    }
                }
            }
            return true;
        }
    }
}
