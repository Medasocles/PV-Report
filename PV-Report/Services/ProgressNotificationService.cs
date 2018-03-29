using System;

namespace PvReport.Services
{
    public class ProgressNotificationService
    {
        public event EventHandler<string> ProgressChanged;

        public void Notify(string message)
        {
            ProgressChanged?.Invoke(this, message);
        }
    }
}
