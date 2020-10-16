using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace.Text {
    public class Scheduler : MonoBehaviour {
        public static Scheduler     Single;
        private       StationLogger StationLogger = StationLogger.Single;

        #region time management

        // base and emergency
        private float MinWaitTime = 0.3f;

        // warnings
        private float MeanWarnTime  = 1.5f;
        private float MaxWarnTime   = 3f;
        private float WarnCounter   = 0f;
        private float WarnThreshold = 0f;

        // info messages
        private float MeanInfoTime  = 2.5f;
        private float MaxInfoTime   = 6f;
        private float InfoCounter   = 0f;
        private float InfoThreshold = 0f;

        // scheduled update messages
        private float UpdateCounter   = 0f;
        private float UpdateThreshold = 4f;

        #endregion

        #region Announced Importants and Warnings

        private List<IAlertReplacements> myEmergencies = new List<IAlertReplacements>();
        private List<IAlertReplacements> myWarnings    = new List<IAlertReplacements>();

        #endregion

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        public void ReportImportant(StationAlertType alertType, params IAlertReplacements[] replacementSources) {
            StationLogger.Alert(alertType, replacementSources);
        }

        public void ReportWarning(StationAlertType alertType, params IAlertReplacements[] replacementSources) {
            StationLogger.Alert(alertType, replacementSources);
        }
    }
}