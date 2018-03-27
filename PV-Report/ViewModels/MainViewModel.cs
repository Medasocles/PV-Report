using PvReport.Library.MVVM.ViewModelBase;

namespace PvReport.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private SyncPvReportsViewModel _syncPvReportsViewModel;

        public MainViewModel()
        {
            SyncPvReportsViewModel = new SyncPvReportsViewModel();
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
    }
}
