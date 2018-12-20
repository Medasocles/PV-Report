using PvReport.Models;
using System;
using System.Globalization;

namespace PvReport.Converter
{
    public class PvReportRangeTypeToTimeRangeTextConverter : BaseValueConverter<PvReportRangeTypeToTimeRangeTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PvReportRangeModel rangeModel)
            {
                if (rangeModel.RangeType == ReportRangeType.Month)
                    return $"{rangeModel.From:MMMM}";
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
