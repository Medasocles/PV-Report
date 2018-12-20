using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace PvReport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");
        }
        private void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}
