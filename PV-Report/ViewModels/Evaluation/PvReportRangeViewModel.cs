using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using System;
using System.Linq;

namespace PvReport.ViewModels.Evaluation
{
    public class PvReportRangeViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private int _year;
        private double _production;
        private double _consumption;
        private double _selfConsumption;
        private double _gridFeedIn;
        private double _gridTakeOut;
        private DateTime _from;
        private DateTime _to;

        public PvReportRangeViewModel(int year, PvReportService pvReportService)
        {
            _pvReportService = pvReportService;
            _pvReportService.PvReports.CollectionChanged += OnReportsChanged;
            Year = year;
            UpdateReports();
        }

        private void OnReportsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateReports();
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        public DateTime From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged();
            }
        }

        public DateTime To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gesamt Erzeugung
        /// </summary>
        public double Production
        {
            get => _production;
            set
            {
                _production = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gesamt Verbrauch
        /// </summary>
        public double Consumption
        {
            get => _consumption;
            set
            {
                _consumption = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Eigenverbrauch
        /// </summary>
        public double SelfConsumption
        {
            get => _selfConsumption;
            set
            {
                _selfConsumption = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Energie ins Netz eingespeist
        /// </summary>
        public double GridFeedIn
        {
            get => _gridFeedIn;
            set
            {
                _gridFeedIn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Energie vom Netz bezogen
        /// </summary>
        public double GridTakeOut
        {
            get => _gridTakeOut;
            set
            {
                _gridTakeOut = value;
                OnPropertyChanged();
            }
        }

        private void UpdateReports()
        {
            var reports = _pvReportService.PvReports.Where(report => report.Date.Year.Equals(Year)).ToList();
            Production = reports.Sum(report => report.TotalProduction) / 1000;
            Consumption = reports.Sum(report => report.TotalConsumption) / 1000;
            SelfConsumption = reports.Sum(report => report.SelfConsumption) / 1000;
            GridFeedIn = reports.Sum(report => report.GridFeedIn) / 1000;
            GridTakeOut = reports.Sum(report => report.GridTakeOut) / 1000;
            From = reports.Min(report => report.Date);
            To = reports.Max(report => report.Date);
        }
    }
}
