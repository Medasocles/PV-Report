using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PvReport.ViewModels.Evaluation
{
    public class YearsTabViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private int _selectedYear;
        private readonly IDictionary<int, YearSummaryViewModel> _yearSummaryVms;
        private YearSummaryViewModel _activeYearVm;

        public YearsTabViewModel(PvReportService pvReportService)
        {
            _pvReportService = pvReportService;
            _pvReportService.PvReports.CollectionChanged += OnPvReportsChanged;

            _yearSummaryVms = new Dictionary<int, YearSummaryViewModel>();

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
            get => _activeYearVm;
            set
            {
                _activeYearVm = value;
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
            if (!_yearSummaryVms.ContainsKey(year))
            {
                var vm = new YearSummaryViewModel(year, _pvReportService);
                _yearSummaryVms.Add(year, vm);
            }

            ActiveYearSummaryViewModel = _yearSummaryVms[year];
        }

        private void OnPvReportsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateYears();
        }
    }
}
