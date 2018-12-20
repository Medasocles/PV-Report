using PvReport.Services;
using System;

namespace PvReport.Models
{
    public enum ReportRangeType
    {
        None,
        Day,
        Month, // calender month 1.-[endOfMonth]
        Year, // calender year 1.1-31.12.
        Dynamic // a custom date/time range
    }

    // Datum und Uhrzeit,Gesamt Erzeugung,Gesamt Verbrauch,Eigenverbrauch,Energie ins Netz eingespeist,Energie vom Netz bezogen,
    // [dd.MM.yyyy],[Wh],[Wh],[Wh],[Wh],[Wh],
    // 08.08.2017,33481.52,13788.33,9293.52,24246.61,4567.15,

    public class PvReportRangeModel : PvReportModelBase
    {
        public PvReportRangeModel()
        {
            From = DateTime.MinValue;
            To = DateTime.MinValue;
        }

        /// <summary>
        /// Start of span
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// End of span
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// Type of time span this model represents.
        /// E.g. only a day, a whole month or year in calender or a dynamic range
        /// </summary>
        public ReportRangeType RangeType => PvReportRangeTypeCalculator.CalulateRangeType(From, To);
    }
}
