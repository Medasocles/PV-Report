using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using PvReport.ViewModels.Evaluation;

namespace PvReport.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PvReportService _pvReportService;
        private ProgressNotificationService _progressNotificationService;
        private SyncPvReportsViewModel _syncPvReportsViewModel;
        private ProgressVisualizationViewModel _progressVisualizationViewModel;
        private YearSummaryViewModel _yearSummaryViewModel;

        public MainViewModel()
        {
            _pvReportService = new PvReportService();
            _pvReportService.Initialize();
            _progressNotificationService = new ProgressNotificationService();
            SyncPvReportsViewModel = new SyncPvReportsViewModel(_pvReportService, _progressNotificationService);
            ProgressVisualizationViewModel = new ProgressVisualizationViewModel(_progressNotificationService);
            YearSummaryViewModel = new YearSummaryViewModel(_pvReportService);
        }

        public SyncPvReportsViewModel SyncPvReportsViewModel
        {
            get => _syncPvReportsViewModel;
            set
            {
                _syncPvReportsViewModel = value;
                OnPropertyChanged();
            }
        }

        public ProgressVisualizationViewModel ProgressVisualizationViewModel
        {
            get => _progressVisualizationViewModel;
            set
            {
                _progressVisualizationViewModel = value;
                OnPropertyChanged();
            }
        }

        public YearSummaryViewModel YearSummaryViewModel
        {
            get => _yearSummaryViewModel;
            set
            {
                _yearSummaryViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
