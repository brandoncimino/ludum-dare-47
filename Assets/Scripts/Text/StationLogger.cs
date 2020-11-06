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

        public  AlertRenderer       AlertPrefab;
        private List<AlertRenderer> Alerts    = new List<AlertRenderer>();
        private int                 maxAlerts = 5;

        #endregion

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// creates one combined alert for all alertTypes in the given list according to the order they have in the list.
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="severity"></param>
        /// <param name="replacementSources"></param>
        public static void Alert(
            List<StationAlertType> alertType,
            Alert.SeverityLevel severity,
            params IAlertReplacements[] replacementSources
        ) {
            var message = GenerateAlertMessage(alertType, replacementSources);

            if (Single.StationLogGroup) {
                var newAlert = Single.CreateAlert(); // newAlert is an instance of the AlertRenderer class
                newAlert.Alert = new Alert {
                    AlertMessage = message,
                    Severity     = severity,
                    TimeStamp    = DateTime.Now
                };
            }
        }

        /// <summary>
        /// creates alert for a single given alertType
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="severity"></param>
        /// <param name="replacementSources"></param>
        public static void Alert(
            StationAlertType alertType,
            Alert.SeverityLevel severity,
            params IAlertReplacements[] replacementSources
        ) {
            Alert(new List<StationAlertType>() {alertType}, severity, replacementSources);
        }

        /// <summary>
        /// chooses random message phrases according to the given list of alert types, replaces placeholder names and joins the messages together in the order they are in the list.
        /// </summary>
        /// <param name="alertTypeList">List of all the alert types to be put into one alert</param>
        /// <param name="replacementSources"></param>
        /// <returns></returns>
        private static string GenerateAlertMessage(
            List<StationAlertType> alertTypeList,
            IAlertReplacements[] replacementSources
        ) {
            string message = "";
            foreach (var alertType in alertTypeList) {
                try {
                    var alert = Single.StationLog.Alerts[alertType].Random();
                    message = String.Join("", message, StationLog.FormatAlert(alert, replacementSources));
                }
                catch (KeyNotFoundException) {
                    var error = $"Missing {alertType.GetType().Name}".Colorize(Color.red);
                    message = String.Join(" ", message, $"{error}: {alertType}");
                }
            }

            return message;
        }

        /// <summary>
        /// chooses a random message phrase based on the given alert type and replaces the placeholder name.
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="replacementSources"></param>
        /// <returns></returns>
        private static string GenerateAlertMessage(
            StationAlertType alertType,
            IAlertReplacements[] replacementSources
        ) {
            return GenerateAlertMessage(new List<StationAlertType>() {alertType}, replacementSources);
        }

        /// <summary>
        /// chooses a random soup. Is it yummy? Ask the chef.
        /// </summary>
        /// <remarks>
        /// As of 10/15/2020, this is NOT actually based on the day/night cycle.
        /// </remarks>
        /// <returns></returns>
        public static Soup SoupOfTheDay => Single.SoupDatabase.Soups.Random();

        private AlertRenderer CreateAlert() {
            // create the alert object and add it to the ones we already have
            var newAlertRenderer = Instantiate(AlertPrefab, StationLogGroup.transform);
            Alerts.Add(newAlertRenderer);

            // check if max alert amount has been reached, and delete oldest one if so
            if (Alerts.Count > maxAlerts) {
                var oldAlert = Alerts[0];
                Alerts.Remove(oldAlert);
                Destroy(oldAlert.gameObject);
            }

            return newAlertRenderer;
        }
    }
}