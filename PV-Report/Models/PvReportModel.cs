using PvReport.Library.MVVM.ModelBase;
using System;

namespace PvReport.Models
{

    // Datum und Uhrzeit,Gesamt Erzeugung,Gesamt Verbrauch,Eigenverbrauch,Energie ins Netz eingespeist,Energie vom Netz bezogen,
    // [dd.MM.yyyy],[Wh],[Wh],[Wh],[Wh],[Wh],
    // 08.08.2017,33481.52,13788.33,9293.52,24246.61,4567.15,

    [Serializable]
    public class PvReportModel : ModelBase
    {
        public DateTime Date { get; set; }

        /// <summary>
        /// Gesamt Erzeugung
        /// </summary>
        public double TotalProduction { get; set; }

        /// <summary>
        /// Gesamt Verbrauch
        /// </summary>
        public double TotalConsumption { get; set; }

        /// <summary>
        /// Eigenverbrauch
        /// </summary>
        public double SelfConsumption { get; set; }

        /// <summary>
        /// Energie ins Netz eingespeist
        /// </summary>
        public double GridFeedIn { get; set; }

        /// <summary>
        /// Energie vom Netz bezogen
        /// </summary>
        public double GridTakeOut { get; set; } 
    }
}
