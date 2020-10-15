using Newtonsoft.Json;

using Packages.BrandonUtils.Runtime.Collections;

using TMPro;

using UnityEngine;

namespace DefaultNamespace.Text {
    public class StationLogger : MonoBehaviour {
        public TextAsset StationLogAsset;
        public TMP_Text  StationLogMesh;

        private StationLog _StationLog;
        private StationLog StationLog =>
            _StationLog ?? (_StationLog = JsonConvert.DeserializeObject<StationLog>(StationLogAsset.text));

        public static StationLogger Single;

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        public static void Log(StationAlertType alertType, CreatureAI astronaut = null) {
            var alert     = Single.StationLog.Alerts[alertType].Random();
            var formatted = StationLog.FormatAlert(alert, astronaut);
            Single.StationLogMesh.text += $"\n{formatted}";
        }
    }
}