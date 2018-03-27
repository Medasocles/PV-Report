using PvReport.Library.MVVM.ModelBase;
using System;

namespace PvReport.Models
{
    [Serializable]
    public class SynchronizationInfo : ModelBase
    {
        private DateTime _lastSyncDate;

        public DateTime LastSyncDate
        {
            get => _lastSyncDate;
            set
            {
                _lastSyncDate = value;
                OnPropertyChanged();
            }
        }

        public static SynchronizationInfo Empty => new SynchronizationInfo() { LastSyncDate = new DateTime(2014, 12, 21) };
    }
}
