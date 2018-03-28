using PvReport.Library.MVVM.ModelBase;
using System;

namespace PvReport.Models
{
    [Serializable]
    public class SyncSettingsModel : ModelBase
    {
        private DateTime _lastSyncDate;
        private string _userName;
        private string _password;
        private string _serverAddressName;
        private int _serverPort;

        public DateTime LastSyncDate
        {
            get => _lastSyncDate;
            set
            {
                _lastSyncDate = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ServerAddressName
        {
            get => _serverAddressName;
            set
            {
                _serverAddressName = value;
                OnPropertyChanged();
            }
        }

        public int ServerPort
        {
            get => _serverPort;
            set
            {
                _serverPort = value;
                OnPropertyChanged();
            }
        }

        public static SyncSettingsModel Default => new SyncSettingsModel() { LastSyncDate = new DateTime(2014, 12, 21), ServerAddressName = "imap.gmail.com", ServerPort = 993 };
    }
}
