using MimeKit;
using PvReport.Library.Command;
using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;
using PvReport.Services.Mail;
using PvReport.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PvReport.ViewModels
{
    public class SyncPvReportsViewModel : ViewModelBase
    {
        private SyncSettingsModel _syncSettingsModel;

        public SyncPvReportsViewModel()
        {
            SyncSettingsModel = StorageService.LoadSynchronizationInfo();

            SyncReportsCommand = new Command<object>(OnSyncReportsCommandExecute, OnSyncReportsCommandCanExecute);
        }

        private bool OnSyncReportsCommandCanExecute(object o)
        {
            return !string.IsNullOrWhiteSpace(SyncSettingsModel?.UserName) && !string.IsNullOrWhiteSpace(SyncSettingsModel.Password);
        }

        public ICommand SyncReportsCommand { get; }

        public SyncSettingsModel SyncSettingsModel
        {
            get => _syncSettingsModel;
            set
            {
                if (_syncSettingsModel != null)
                    _syncSettingsModel.PropertyChanged -= OnSyncSettingsModelPropertyChanged;

                _syncSettingsModel = value;

                if (_syncSettingsModel != null)
                {
                    _syncSettingsModel.PropertyChanged += OnSyncSettingsModelPropertyChanged;
                    StorageService.SaveSynchronizationInfo(_syncSettingsModel);
                }

                OnPropertyChanged();
            }
        }

        private void OnSyncSettingsModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StorageService.SaveSynchronizationInfo(_syncSettingsModel);
        }

        private async void OnSyncReportsCommandExecute(object o)
        {
            if (await SynchronizeWithOnlineMailRepositoryAsync() is IEnumerable<MimeMessage> messages)
            {
                PvReportMailParser.Parse(messages);
            }

            // debug:
            var pvReportDownloadModels = PvReportMailParser.Parse(StorageService.LoadMimeMessages());
            PvReportDownloader.DownloadReports(StorageService.PvReportsRepository, pvReportDownloadModels);
        }

        private async Task<IEnumerable<MimeMessage>> SynchronizeWithOnlineMailRepositoryAsync()
        {
            try
            {
                var mailRepository = new MailRepository(SyncSettingsModel.ServerAddressName, SyncSettingsModel.ServerPort, true, SyncSettingsModel.UserName, SyncSettingsModel.Password);
                var allEmails = await Task.Run(() => mailRepository.GetAllMails(SyncSettingsModel.LastSyncDate, "PV Report"));
                var allMailArray = allEmails as MimeMessage[] ?? allEmails.ToArray();
                if (allMailArray.Any())
                {
                    SyncSettingsModel.LastSyncDate = DateTime.Now;
                    StorageService.SaveMimeMessages(allMailArray);
                }

                return allMailArray;
            }
            catch (Exception e)
            { }

            return null;
        }
    }
}
