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
        private AvailableYearsViewModel _availableYearsViewModel;

        public MainViewModel()
        {
            _pvReportService = new PvReportService();
            _pvReportService.Initialize();
            _progressNotificationService = new ProgressNotificationService();
            SyncPvReportsViewModel = new SyncPvReportsViewModel(_pvReportService, _progressNotificationService);
            ProgressVisualizationViewModel = new ProgressVisualizationViewModel(_progressNotificationService);
            AvailableYearsViewModel = new AvailableYearsViewModel(_pvReportService);
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

        public AvailableYearsViewModel AvailableYearsViewModel
        {
            get => _availableYearsViewModel;
            set
            {
                _availableYearsViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
