using PvReport.Library.MVVM.ModelBase;
using System;

namespace PvReport.Models
{
    [Serializable]
    public class PvReportDownloadInfo : ModelBase
    {
        // fields of the email table containing the reports download info
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string Intervall { get; set; }
        public string FileFormat { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string DownloadLink { get; set; }

        // additional properties
        public bool Downloaded { get; set; }
        public bool DownloadError { get; set; }
        public Exception DownloadException { get; set; }
    }
}
