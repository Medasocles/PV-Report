using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;

namespace PvReport.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PvReportService _pvReportService;
        private ProgressNotificationService _progressNotificationService;
        private SyncPvReportsViewModel _syncPvReportsViewModel;
        private ProgressVisualizationViewModel _progressVisualizationViewModel;

        public MainViewModel()
        {
            _pvReportService = new PvReportService();
            _progressNotificationService = new ProgressNotificationService();
            SyncPvReportsViewModel = new SyncPvReportsViewModel(_pvReportService, _progressNotificationService);
            ProgressVisualizationViewModel = new ProgressVisualizationViewModel(_progressNotificationService);
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
    }
}
