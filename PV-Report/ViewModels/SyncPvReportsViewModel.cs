using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;

namespace PvReport.ViewModels
{
    public class SyncPvReportsViewModel : ViewModelBase
    {
        private SynchronizationInfo _synchronizationInfo;

        public SyncPvReportsViewModel()
        {
            SynchronizationInfo = RepositoryService.LoadSynchronizationInfo();
        }

        public SynchronizationInfo SynchronizationInfo
        {
            get => _synchronizationInfo;
            set
            {
                _synchronizationInfo = value;
                OnPropertyChanged();
            }
        }
    }
}
