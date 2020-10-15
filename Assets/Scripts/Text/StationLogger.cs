using System.Collections.Generic;

using Newtonsoft.Json;

using Packages.BrandonUtils.Runtime;
using Packages.BrandonUtils.Runtime.Collections;

using TMPro;

using UnityEngine;

namespace DefaultNamespace.Text {
    public class StationLogger : MonoBehaviour {
        public TextAsset StationLogAsset;
        public TextAsset SoupDatabaseAsset;
        public TMP_Text  StationLogMesh;

        private StationLog _StationLog;
        private StationLog StationLog =>
            _StationLog ?? (_StationLog = JsonConvert.DeserializeObject<StationLog>(StationLogAsset.text));

        public static StationLogger Single;

        private SoupDatabase _SoupDatabase;
        public SoupDatabase SoupDatabase => _SoupDatabase ??
                                            (_SoupDatabase =
                                                 JsonConvert.DeserializeObject<SoupDatabase>(SoupDatabaseAsset.text));

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        public static void Log(StationAlertType alertType, params IAlertReplacements[] replacementSources) {
            string message;
            try {
                var alert = Single.StationLog.Alerts[alertType].Random();
                message = StationLog.FormatAlert(alert, replacementSources);
            }
            catch (KeyNotFoundException e) {
                var error = $"Missing {alertType.GetType().Name}".Colorize(Color.red);
                message = $"{error}: {alertType}";
            }

            Single.StationLogMesh.text += $"\n{message}";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// As of 10/15/2020, this is NOT actually based on the day/night cycle.
        /// </remarks>
        /// <returns></returns>
        public static Soup SoupOfTheDay => Single.SoupDatabase.Soups.Random();
    }
}