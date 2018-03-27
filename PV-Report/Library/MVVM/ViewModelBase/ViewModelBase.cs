using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PvReport.Library.MVVM.ViewModelBase
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region property changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
