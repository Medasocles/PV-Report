using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;
using System.Collections.ObjectModel;

namespace PvReport.ViewModels.Evaluation
{
    public class YearSummaryViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private PvReportRangeModel _yearSummary;

        public YearSummaryViewModel(int year, PvReportService pvReportService)
        {
            Year = year;
            _pvReportService = pvReportService;
            _pvReportService.PvReports.CollectionChanged += OnPvReportsCollectionChanged;

            Months = new ObservableCollection<PvReportRangeModel>();
            Initialize();
        }

        public int Year { get; }

        public ObservableCollection<PvReportRangeModel> Months { get; }
        
        public PvReportRangeModel YearSummary
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
