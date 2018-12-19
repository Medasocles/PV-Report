using System;

namespace PvReport.Models
{

    // Datum und Uhrzeit,Gesamt Erzeugung,Gesamt Verbrauch,Eigenverbrauch,Energie ins Netz eingespeist,Energie vom Netz bezogen,
    // [dd.MM.yyyy],[Wh],[Wh],[Wh],[Wh],[Wh],
    // 08.08.2017,33481.52,13788.33,9293.52,24246.61,4567.15,

    public class PvReportSpanModel : PvReportModelBase
    {
        /// <summary>
        /// Start of span
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// End of span
        /// </summary>
        public DateTime To { get; set; }
    }
}
