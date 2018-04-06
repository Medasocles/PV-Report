using MimeKit;
using PvReport.Library.Command;
using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Models;
using PvReport.Services;
using PvReport.Services.Mail;
using PvReport.Services.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PvReport.ViewModels
{
    public class SyncPvReportsViewModel : ViewModelBase
    {
        private readonly PvReportService _pvReportService;
        private readonly ProgressNotificationService _progressNotificationService;
        private SyncSettingsModel _syncSettingsModel;

        public SyncPvReportsViewModel()
        {
            
        }

        public SyncPvReportsViewModel(PvReportService pvReportService,
            ProgressNotificationService progressNotificationService)
        {
            _pvReportService = pvReportService;
            _progressNotificationService = progressNotificationService;
            SyncSettingsModel = StorageService.LoadSynchronizationInfo();

            SyncReportsCommand = new Command<object>(OnSyncReportsCommandExecute, OnSyncReportsCommandCanExecute);
            OpenRepositoryFolderCommand = new Command<object>(OnOpenRepostoryFolderCommandExecute);
        }

        public ICommand SyncReportsCommand { get; }
        public ICommand OpenRepositoryFolderCommand { get; }

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

        private bool OnSyncReportsCommandCanExecute(object o)
        {
            return !string.IsNullOrWhiteSpace(SyncSettingsModel?.UserName) && !string.IsNullOrWhiteSpace(SyncSettingsModel.Password);
        }

        private async void OnSyncReportsCommandExecute(object o)
        {
            _progressNotificationService.Notify("Starte Synchronisation...");
            // synchronize with online pv-reports from mail-repo
            if (await DownloadLatestReportMailsAsync() is IEnumerable<MimeMessage> messages)
            {
                // parse the downloaded mails for download links
                var newDownloadInfos = await PvReportMailParser.ParseAsync(messages);
                
                // download the report-files
                await PvReportDownloader.DownloadReportsAsync(newDownloadInfos, _progressNotificationService);

                _progressNotificationService.Notify("Synchronisation abgeschlossen.");
            }

            // load reports
            _pvReportService.LoadPvReports();

            // set sync date
            SyncSettingsModel.LastSyncDate = _pvReportService.PvReports.Max(report => report.Date);
        }

        private void OnOpenRepostoryFolderCommandExecute(object o)
        {
            Process.Start(StorageService.RepositoryRootFolder);
        }

        /// <summary>
        /// Download all Report-EMails after the LastSyncDate with gmail-label 'PV Report'
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<MimeMessage>> DownloadLatestReportMailsAsync()
        {
            try
            {
                var mailRepository = new MailRepository(SyncSettingsModel.ServerAddressName,
                    SyncSettingsModel.ServerPort, true, SyncSettingsModel.UserName, SyncSettingsModel.Password,
                    _progressNotificationService);
                var allEmails = await Task.Run(() => mailRepository.GetAllMails(SyncSettingsModel.LastSyncDate, "PV Report"));
                var allMailArray = allEmails as MimeMessage[] ?? allEmails.ToArray();
                if (allMailArray.Any())
                {
                    _progressNotificationService.Notify("Speichere heruntergeladene EMails.");
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
