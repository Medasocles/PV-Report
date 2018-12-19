using System;
using System.Collections.ObjectModel;
using System.Linq;
using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;

namespace PvReport.ViewModels.Evaluation
{
    public class YearSummaryViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private PvReportSpanModel _yearSummary;

        public YearSummaryViewModel(int year, PvReportService pvReportService)
        {
            Year = year;
            _pvReportService = pvReportService;
            _pvReportService.PvReports.CollectionChanged += OnPvReportsCollectionChanged;

            Months = new ObservableCollection<PvReportSpanModel>();
            Initialize();
        }

        public int Year { get; }

        public ObservableCollection<PvReportSpanModel> Months { get; }
        
        public PvReportSpanModel YearSummary
        {
            get => _yearSummary;
            set
            {
                _yearSummary = value; 
                OnPropertyChanged();
            }
        }

        private void Initialize()
        {
            YearSummary = _pvReportService.GetPvReportYearSummary(Year);

            for (var i = 1; i <= 12; i++)
            {
                Months.Add(_pvReportService.GetPvReportMonthSummary(Year, i));
            }
        }

        private void OnPvReportsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }
    }
}
