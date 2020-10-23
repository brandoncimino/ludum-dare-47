using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Packages.BrandonUtils.Runtime.Collections;
using Packages.BrandonUtils.Runtime.Strings;

using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Text {
    public class StationLogger : MonoBehaviour {
        public static StationLogger       Single;
        public        VerticalLayoutGroup StationLogGroup;

        #region Text Assets

        #region StationLog

        public  TextAsset  StationLogAsset;
        private StationLog _StationLog;
        private StationLog StationLog =>
            _StationLog ?? (_StationLog = JsonConvert.DeserializeObject<StationLog>(StationLogAsset.text));

        #endregion

        #region SoupDatabase

        public  TextAsset    SoupDatabaseAsset;
        private SoupDatabase _SoupDatabase;
        public SoupDatabase SoupDatabase => _SoupDatabase ??
                                            (_SoupDatabase =
                                                 JsonConvert.DeserializeObject<SoupDatabase>(SoupDatabaseAsset.text));

        #endregion

        #endregion

        #region Alerts

        public  AlertRenderer AlertPrefab;
        private List<Alert>   Alerts;

        #endregion

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        public static void Alert(
            StationAlertType alertType,
            Alert.SeverityLevel severity,
            params IAlertReplacements[] replacementSources
        ) {
            var message = GenerateAlertMessage(alertType, replacementSources);

            if (Single.StationLogGroup) {
                var newAlert = Single.CreateAlert();
                newAlert.Alert = new Alert {
                    AlertMessage = message,
                    Severity     = severity,
                    TimeStamp    = DateTime.Now
                };
            }
        }

        private static string GenerateAlertMessage(
            StationAlertType alertType,
            IAlertReplacements[] replacementSources
        ) {
            string message;
            try {
                var alert = Single.StationLog.Alerts[alertType].Random();
                message = StationLog.FormatAlert(alert, replacementSources);
            }
            catch (KeyNotFoundException) {
                var error = $"Missing {alertType.GetType().Name}".Colorize(Color.red);
                message = $"{error}: {alertType}";
            }

            return message;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// As of 10/15/2020, this is NOT actually based on the day/night cycle.
        /// </remarks>
        /// <returns></returns>
        public static Soup SoupOfTheDay => Single.SoupDatabase.Soups.Random();

        private AlertRenderer CreateAlert() {
            return Instantiate(AlertPrefab, StationLogGroup.transform);
        }
    }
}