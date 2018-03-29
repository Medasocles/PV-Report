using PvReport.Library.MVVM.ViewModelBase;
using PvReport.Services;
using System;

namespace PvReport.ViewModels
{
    public class ProgressVisualizationViewModel : ViewModelBase
    {
        private readonly ProgressNotificationService _progressNotificationService;
        private string _progressMessage;

        public ProgressVisualizationViewModel(ProgressNotificationService progressNotificationService)
        {
            _progressNotificationService = progressNotificationService;
            _progressNotificationService.ProgressChanged += OnProgressChanged;
        }

        public string ProgressMessage
        {
            get => _progressMessage;
            set
            {
                _progressMessage = value;
                OnPropertyChanged();
            }
        }

        private void OnProgressChanged(object sender, string message)
        {
            ProgressMessage = message;
        }
    }
}
