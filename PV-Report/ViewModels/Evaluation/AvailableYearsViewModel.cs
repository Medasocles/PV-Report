using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PvReport.ViewModels.Evaluation
{
    public class AvailableYearsViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private int _selectedYear;
        private IDictionary<int, YearSummaryViewModel> _yearSummaryViewModels;
        private YearSummaryViewModel _activeYearSummaryViewModel;

        public AvailableYearsViewModel(PvReportService pvReportService)
        {
            _pvReportService = pvReportService;
            _pvReportService.PvReports.CollectionChanged += OnPvReportsChanged;

            _yearSummaryViewModels = new Dictionary<int, YearSummaryViewModel>();

            Years = new ObservableCollection<int>();
            UpdateYears();

            if (Years.Count > 0)
                SelectedYear = Years.Max();
        }

        public ObservableCollection<int> Years { get; }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                SelectYearSummaryViewModel(value);
            }
        }

        public YearSummaryViewModel ActiveYearSummaryViewModel
        {
            get => _activeYearSummaryViewModel;
            set
            {
                _activeYearSummaryViewModel = value;
                OnPropertyChanged();
            }
        }

        private void UpdateYears()
        {
            foreach (var year in _pvReportService.PvReports.Select(report => report.Date.Year).Distinct())
            {
                if (!Years.Contains(year))
                    Years.Add(year);
            }
        }

        private void SelectYearSummaryViewModel(int year)
        {
            if (!_yearSummaryViewModels.ContainsKey(year))
            {
                var vm = new YearSummaryViewModel(year, _pvReportService);
                _yearSummaryViewModels.Add(year, vm);
            }

            ActiveYearSummaryViewModel = _yearSummaryViewModels[year];
        }

        private void OnPvReportsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateYears();
        }
    }
}
