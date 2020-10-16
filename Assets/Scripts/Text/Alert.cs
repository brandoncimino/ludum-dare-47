using System;

using UnityEngine;

namespace DefaultNamespace.Text {
    public class Alert {
        /// <summary>
        /// The "severity" of the alert.
        /// </summary>
        /// <remarks>
        /// Roughly corresponds to "logging levels", i.e. <see cref="Debug.Log(object)"/>, <see cref="Debug.LogWarning(object)"/>, etc.
        /// </remarks>
        public enum SeverityLevel {
            Info,
            Warning,
            Emergency
        }

        public StationAlertType AlertType;
        public SeverityLevel    Severity;

        /// <summary>
        /// The formatted message returned by <see cref="StationLog.FormatAlert(string,System.Collections.Generic.Dictionary{string,string})"/>.
        /// </summary>
        public string AlertMessage;

        public DateTime TimeStamp;

        private const string TimeStampFormat = "m:ss";
    }
}