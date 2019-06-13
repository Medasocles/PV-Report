using PvReport.Models;
using PvReport.Services.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace PvReport.Services
{
    public class PvReportService
    {
        public PvReportService()
        {
            PvReports = new ObservableCollection<PvReportModel>();
        }

        public readonly ObservableCollection<PvReportModel> PvReports; // only balance-reports not production-reports

        public bool LoadPvReports()
        {
            PvReports.Clear();

            // Datum und Uhrzeit,Gesamt Erzeugung,Gesamt Verbrauch,Eigenverbrauch,Energie ins Netz eingespeist,Energie vom Netz bezogen,
            // [dd.MM.yyyy],[Wh],[Wh],[Wh],[Wh],[Wh],
            // 08.08.2017,33481.52,13788.33,9293.52,24246.61,4567.15,

            foreach (var contentList in StorageService.LoadPvReportContents())
            {
                // only daily reports are currently used
                var contentArray = contentList as string[] ?? contentList.ToArray();
                if (contentArray.Length == 3)
                {
                    var tokens = contentArray[2].Split(',');
                    var date = DateTime.Parse(tokens[0]);
                    if (!PvReports.Any(report => report.Date.Equals(date)))
                    {
                        PvReports.Add(new PvReportModel
                        {
                            Date = date,
                            TotalProduction = double.Parse(tokens[1], CultureInfo.InvariantCulture),
                            TotalConsumption = double.Parse(tokens[2], CultureInfo.InvariantCulture),
                            SelfConsumption = double.Parse(tokens[3], CultureInfo.InvariantCulture),
                            GridFeedIn = double.Parse(tokens[4], CultureInfo.InvariantCulture),
                            GridTakeOut = double.Parse(tokens[5], CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            return true;
        }


        public PvReportRangeModel GetPvReportYearSummary(int year)
        {
            var reports = PvReports.Where(report => report.Date.Year.Equals(year)).ToList();
            return !reports.Any() ? null : AggregatePvReports(reports);
        }

        public PvReportRangeModel GetPvReportMonthSummary(int year, int month)
        {
            var reports = PvReports.Where(report => report.Date.Year.Equals(year) && report.Date.Month.Equals(month)).ToList();

            if (reports.Any())
                return AggregatePvReports(reports);

            else
            {
                return new PvReportRangeModel()
                {
                    From = new DateTime(year, month, 1),
                    To = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                };
            }

            return reports.Any() ? null : AggregatePvReports(reports);
        }

        public PvReportRangeModel GetReportSpan(DateTime from, DateTime to)
        {
            var reports = PvReports.Where(report => report.Date >= from && report.Date <= to).ToList();
            return AggregatePvReports(reports);
        }

        private static PvReportRangeModel AggregatePvReports(IList<PvReportModel> reports)
        {
            var spanModel = new PvReportRangeModel
            {
                TotalProduction = reports.Sum(report => report.TotalProduction) / 1000,
                TotalConsumption = reports.Sum(report => report.TotalConsumption) / 1000,
                SelfConsumption = reports.Sum(report => report.SelfConsumption) / 1000,
                GridFeedIn = reports.Sum(report => report.GridFeedIn) / 1000,
                GridTakeOut = reports.Sum(report => report.GridTakeOut) / 1000,
                From = reports.Min(report => report.Date),
                To = reports.Max(report => report.Date),
            };

            return spanModel;
        }
    }
}
