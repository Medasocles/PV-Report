using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PvReport.Library.MVVM.ModelBase
{
    public abstract class ModelBase : INotifyPropertyChanged
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
