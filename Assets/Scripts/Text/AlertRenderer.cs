using System.Collections.Generic;

using Packages.BrandonUtils.Runtime;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Text {
    public class AlertRenderer : MonoBehaviour {
        public TMP_Text Mesh;
        public Image    Background;
        public Alert    Alert;

        #region Severity

        [Header("Severity Colors")]
        public List<Color> SeverityColors;

        #endregion

        [EditorInvocationButton]
        private void SetReferencesToDefault() {
            Mesh       = GetComponentInChildren<TMP_Text>();
            Background = GetComponentInChildren<Image>();
        }

        private void UpdateRenderer() {
            Mesh.text = Alert.AlertMessage;
            //TODO: set the background color based on severity
        }
    }
}