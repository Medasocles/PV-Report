using HtmlAgilityPack;
using MimeKit;
using PvReport.Models;
using PvReport.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PvReport.Services.Mail
{
    public static class PvReportMailParser
    {
        public static IEnumerable<PvReportDownloadInfo> Parse(IEnumerable<MimeMessage> mimeMessages)
        {
            var downloadData = new List<PvReportDownloadInfo>();

            foreach (var mimeMessage in mimeMessages)
            {
                if (mimeMessage.Attachments.Any())
                {
                    ParseAttachments(mimeMessage.Attachments, mimeMessage.Date.DateTime);
                }
                else
                {
                    var data = ParseHtmlBody(mimeMessage.HtmlBody);
                    if (data != null)
                        downloadData.AddRange(data);
                }
            }

            return downloadData;
        }

        private static void ParseAttachments(IEnumerable<MimeEntity> mimeMessageAttachments, DateTime date)
        {
            foreach (var attachmentPart in mimeMessageAttachments.OfType<MimePart>())
            {
                var fileName = attachmentPart.FileName;
                if (fileName.Equals("PV.csv"))
                    fileName = $"Daily-Energiebilanz-Report_Täglicher_Report_{date:dd_MM_yyyy}.csv";

                StorageService.SavePvReport(fileName, attachmentPart.Content);
            }
        }

        /// <example>
        /// Hallo,
        /// im Anhang finden Sie Ihre gewünschten Reports.
        ///
        /// Reporttyp Reportname Intervall Dateiformat Von Bis Download
        /// Energiebilanz PV.Bauer Energiebilanz Daily Daily Csv 08.10.2017 08.10.2017 Download
        /// PV Produktion PV.Bauer Produktion Daily Daily Csv 08.10.2017 08.10.2017 Download
        ///
        /// HINWEIS:
        /// Wir möchten Sie darauf hinweisen, dass das Format des CSV Reports seit dem letzten Solar.web Update nach internationalem Standard abgeändert wurde.Sollten Sie den Report im Excel Format wünschen können Sie das Dateiformat in den Reporteinstellungen im Solar.web Portal ändern.
        ///
        /// Mit freundlichen Grüßen,
        ///
        /// Ihr Solar.web-Team
        /// </example>
        /// <param name="html"></param>
        private static IEnumerable<PvReportDownloadInfo> ParseHtmlBody(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            IList<PvReportDownloadInfo> reportDataModels = null;

            var allRows = htmlDoc.DocumentNode.SelectNodes("//tr");
            if (allRows?.Count == 3)
            {
                reportDataModels = new List<PvReportDownloadInfo>();
                reportDataModels.Add(ConvertTableRowToPvReportDownloadData(allRows[1]));
                reportDataModels.Add(ConvertTableRowToPvReportDownloadData(allRows[2]));
            }

            return reportDataModels;
        }

        private static PvReportDownloadInfo ConvertTableRowToPvReportDownloadData(HtmlNode tableRow)
        {
            var reportDataModel = new PvReportDownloadInfo();
            // each first cell in a table (note: tbody is not always there)
            //var allFirstCells = htmlDoc.DocumentNode.SelectNodes("//tr/td");
            //var allFirstCells = htmlDoc.DocumentNode.SelectNodes("//tr/td")?.Where(td => td.InnerHtml.StartsWith("<a href"));
            var allTableDatas = tableRow.SelectNodes("td");
            if (allTableDatas?.Count == 7)
            {
                reportDataModel.ReportType = allTableDatas[0].InnerText;
                reportDataModel.ReportName = allTableDatas[1].InnerText;
                reportDataModel.Intervall = allTableDatas[2].InnerText;
                reportDataModel.FileFormat = allTableDatas[3].InnerText;
                reportDataModel.From = DateTime.Parse(allTableDatas[4].InnerText);
                reportDataModel.To = DateTime.Parse(allTableDatas[5].InnerText);

                var links = allTableDatas[6].SelectNodes("a");
                if (links.Count == 1)
                {
                    reportDataModel.DownloadLink = links[0].Attributes.AttributesWithName("href")?.FirstOrDefault()?.Value;
                }
            }

            return reportDataModel;
        }
    }
}
