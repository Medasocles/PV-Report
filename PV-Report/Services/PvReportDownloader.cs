using PvReport.Models;
using PvReport.Services.Storage;
using System;
using System.Collections.Generic;
using System.Net;

namespace PvReport.Services
{
    public static class PvReportDownloader
    {
        public static void DownloadReports(IEnumerable<PvReportDownloadInfo> downloadInfos)
        {
            foreach (var downloadInfo in downloadInfos)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadInfo.DownloadLink);

                try
                {
                    using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        var fileName = httpWebResponse.Headers["Content-Disposition"];
                        if (fileName.Contains("*"))
                        {
                            // Example: "attachment; filename*=UTF-8''PV.Bauer_Produktion_Daily_T%C3%A4glicher_Report_2017_10_09.csv"
                            fileName = fileName.Replace("attachment; filename*=", "");
                            var tokens = fileName.Split(new[] { "''" }, StringSplitOptions.RemoveEmptyEntries);
                            fileName = tokens[1];
                            fileName = fileName.Replace("%C3%A4", "ä"); // ä
                            fileName = fileName.Replace("%C3%B6", "ö"); // ö
                            fileName = fileName.Replace("%C3%BC", "ü"); // ü
                            fileName = fileName.Replace("%C3%9F", "ß"); // ß
                        }
                        else
                        {
                            // Example: "attachment; filename = PV.Bauer_Produktion_Monthly_Monatlicher_Report_2017_10.csv"
                            fileName = fileName.Replace("attachment; filename=", "");
                        }

                        //var directory = Path.Combine(storePath, downloadInfo.Date.Year.ToString());
                        //if (!Directory.Exists(directory))
                        //    Directory.CreateDirectory(directory);

                        //var filePath = Path.Combine(storePath, fileName);

                        using (var responseStream = httpWebResponse.GetResponseStream())
                        {
                            StorageService.SavePvReport(fileName, responseStream);

                            //var downBuffer = new byte[1024 * 4];
                            //using (var saveFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                            //    FileShare.ReadWrite))
                            //{
                            //    int receivedBytes;
                            //    while ((receivedBytes = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                            //    {
                            //        saveFileStream.Write(downBuffer, 0, receivedBytes);
                            //        saveFileStream.Flush();
                            //    }
                            //}
                        }
                    }
                }
                catch (Exception e)
                {
                    downloadInfo.DownloadError = true;
                    downloadInfo.DownloadException = e;
                }
            }
        }

        private class SolarWebReportEmail
        {
            public DateTime Date { get; set; }
            public string Subject { get; set; }
            public string FileName { get; set; }
            public List<string> DownloadLinks { get; set; }
            public bool IsRejected { get; set; }

            public List<string> Content { get; set; }

            public bool HasError { get; set; }
            public Exception Exception { get; set; }

        }
    }


}
