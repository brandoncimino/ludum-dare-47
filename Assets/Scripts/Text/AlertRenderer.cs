using Packages.BrandonUtils.Runtime;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Text {
    public class AlertRenderer : MonoBehaviour {
        public TMP_Text Mesh;
        public Image    Background;
        public Alert    Alert;

        public SeverityColorPalette SeverityColorPalette;

        [EditorInvocationButton]
        private void SetReferencesToDefault() {
            Mesh       = GetComponentInChildren<TMP_Text>();
            Background = GetComponentInChildren<Image>();
        }

        private void UpdateRenderer() {
            Mesh.text        = Alert.AlertMessage;
            Background.color = SeverityColorPalette[Alert.Severity];
        }

        private void Update() {
            UpdateRenderer();
        }
    }
}