using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using System.Linq;

namespace PvReport.ViewModels.Evaluation
{
    public class YearSummaryViewModel : ViewModelBase
    {
        public YearSummaryViewModel(PvReportService pvReportService)
        {
            Year = 2017;
            var reports = pvReportService.PvReports.Where(report => report.Date.Year.Equals(2017)).ToList();
            Production = reports.Sum(report => report.TotalProduction) / 1000;
            Consumption = reports.Sum(report => report.TotalConsumption) / 1000;
            SelfConsumption = reports.Sum(report => report.SelfConsumption) / 1000;
            GridFeedIn = reports.Sum(report => report.GridFeedIn) / 1000;
            GridTakeOut = reports.Sum(report => report.GridTakeOut) / 1000;
        }

        public int Year{ get; set; }

        /// <summary>
        /// Gesamt Erzeugung
        /// </summary>
        public double Production { get; set; }

        /// <summary>
        /// Gesamt Verbrauch
        /// </summary>
        public double Consumption { get; set; }

        /// <summary>
        /// Eigenverbrauch
        /// </summary>
        public double SelfConsumption { get; set; }

        /// <summary>
        /// Energie ins Netz eingespeist
        /// </summary>
        public double GridFeedIn { get; set; }

        /// <summary>
        /// Energie vom Netz bezogen
        /// </summary>
        public double GridTakeOut { get; set; }
    }
}
