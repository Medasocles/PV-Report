using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PvReport.Models;

namespace PvReport.Template
{
    internal class PvReportRangeItemTemplateSelector : DataTemplateSelector, INotifyPropertyChanged
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate NullTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is null)
                return NullTemplate;


            if (item is PvReportRangeModel rangeModel)
            {
                return DefaultTemplate;
            }

            return DefaultTemplate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            var parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }
    }


}
