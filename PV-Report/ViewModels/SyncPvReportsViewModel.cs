using PvReport.Library.Command;
using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PvReport.ViewModels
{
    public class SyncPvReportsViewModel : ViewModelBase
    {
        private SynchronizationInfo _synchronizationInfo;

        public SyncPvReportsViewModel()
        {
            SynchronizationInfo = RepositoryService.LoadSynchronizationInfo();

            SyncReportsCommand = new Command<object>(OnSyncReportsCommandExecute);

            // debug code
            RepositoryService.LoadMailsFromStorage();
        }

        public ICommand SyncReportsCommand { get; }

        public SynchronizationInfo SynchronizationInfo
        {
            get => _synchronizationInfo;
            set
            {
                _synchronizationInfo = value;
                OnPropertyChanged();
            }
        }

        private void OnSyncReportsCommandExecute(object o)
        {
            SynchronizeReportMailsAsync();
        }

        private async Task SynchronizeReportMailsAsync()
        {
            var mailRepository = new MailRepository("imap.gmail.com", 993, true, "pvbauer.reports@gmail.com", "melstefbau217");
            var allEmails = await Task.Run(() =>
                mailRepository.GetAllMails(SynchronizationInfo.LastSyncDate, "PV Report"));
            RepositoryService.SaveMailsToFile(allEmails);
        }
    }
}
