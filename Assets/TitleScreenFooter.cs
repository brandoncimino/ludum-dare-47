using Packages.BrandonUtils.Runtime;
using Packages.BrandonUtils.Runtime.UI;

using UnityEngine;
using UnityEngine.UI;

public class TitleScreenFooter : MonoBehaviour {
    private Text _footerText;
    private Text FooterText {
        get {
            if (_footerText == null) {
                _footerText = GetComponent<Text>();
            }

            return _footerText;
        }
    }

    public TextAnchor FooterAnchor;

    public string VersionNumber => $"Version {Application.version}";

    // Start is called before the first frame update
    private void Start() {
        UpdateFooterText();
    }

    [EditorInvocationButton]
    private void UpdateFooter() {
        UpdateFooterText();
        UpdateFooterLocation();
    }

    private void UpdateFooterLocation() {
        FooterText.alignment                      = FooterAnchor;
        FooterText.rectTransform.anchorMax        = FooterAnchor.Anchor();
        FooterText.rectTransform.anchorMin        = FooterAnchor.Anchor();
        FooterText.rectTransform.pivot            = FooterAnchor.Anchor();
        FooterText.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void UpdateFooterText() {
        FooterText.text = VersionNumber;
    }
}