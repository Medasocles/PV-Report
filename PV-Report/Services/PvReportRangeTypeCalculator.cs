using PvReport.Models;
using System;

namespace PvReport.Services
{
    public static class PvReportRangeTypeCalculator
    {
        public static ReportRangeType CalulateRangeType(DateTime start, DateTime end)
        {
            if (start == DateTime.MinValue && end == DateTime.MinValue)
                return ReportRangeType.None;

            if (start.Date == end.Date)
                return ReportRangeType.Day;

            // check if span is a whole month in calender
            if (start.Month == end.Month &&
                start.Year == end.Year &&
                start.Day == 1 && end.Day == DateTime.DaysInMonth(start.Year, start.Month))
                return ReportRangeType.Month;

            // check if span is a whole year in calender
            if (start.Year == end.Year &&
                start.Day == 1 && start.Month == 1 &&
                end.Day == 31 && end.Month == 12)
                return ReportRangeType.Year;

            return ReportRangeType.Dynamic;
        }
    }
}
