using System.Collections.Generic;
using System.Linq;

using Packages.BrandonUtils.Runtime;
using Packages.BrandonUtils.Runtime.Collections;
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

    public string VersionString => $" Version {Application.version} ";

    private static readonly Author Nicole = new Author("Nicole", "Aretz", "Github", "https://github.com/nicolearetz");

    private static readonly Author Brandon = new Author(
        "Brandon",
        "Cimino",
        "brandoncimino.com",
        "http://brandoncimino.com/"
    );

    private static readonly Author Michael = new Author(
        "Michael",
        "Rawls",
        "Github",
        "https://github.com/MichaelBRawls"
    );

    private static Author David = new Author("David", "Rawls", "Github", "https://github.com/GuyWithPasta");

    public List<Author> Authors => new List<Author> {
        Brandon,
        Nicole,
        Michael,
        David
    };

    public string AuthorString => Authors.Select(author => author.Citation()).JoinString(", ");

    // Start is called before the first frame update
    private void Start() {
        UpdateFooter();
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
        var lines = new List<string>()
            {VersionString.Stylize(FontStyle.BoldAndItalic).Colorize(Color.cyan), AuthorString};
        FooterText.text = lines.JoinString("\n");
    }
}