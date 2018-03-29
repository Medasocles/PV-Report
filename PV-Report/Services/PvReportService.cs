using PvReport.Models;
using PvReport.Services.Storage;
using System;
using System.Collections.ObjectModel;
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

                    PvReports.Add(new PvReportModel
                    {
                        Date = DateTime.Parse(tokens[0]),
                        TotalProduction = Convert.ToDouble(tokens[1]),
                        TotalConsumption = Convert.ToDouble(tokens[2]),
                        SelfConsumption = Convert.ToDouble(tokens[3]),
                        GridFeedIn = Convert.ToDouble(tokens[4]),
                        GridTakeOut = Convert.ToDouble(tokens[5])
                    });
                }
            }
            
            return true;
        }
    }
}
