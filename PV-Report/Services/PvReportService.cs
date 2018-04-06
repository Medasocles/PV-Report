using PvReport.Models;
using PvReport.Services.Storage;
using System;
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

        public ObservableCollection<PvReportModel> PvReports; // only balance-reports not production-reports

        public bool LoadPvReports()
        {
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

        public void Initialize()
        {
            LoadPvReports();
        }
    }
}
