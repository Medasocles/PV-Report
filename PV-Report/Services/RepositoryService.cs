using PvReport.Models;
using System.IO;

namespace PvReport.Services
{
    public static class RepositoryService
    {
        private const string ConfigRepository = "Config";
        private const string PvReportsRepository = "PvReports";
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
    }
}
