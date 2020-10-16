using System.Collections.Generic;

using Packages.BrandonUtils.Runtime.Collections;

using UnityEngine;

namespace DefaultNamespace.Text {
    [CreateAssetMenu]
    public class SeverityColorPalette : ScriptableObject {
        public  Color                                  DefaultSeverityColor = Color.clear;
        public  List<Pair<Alert.SeverityLevel, Color>> SeverityColors;
        private Dictionary<Alert.SeverityLevel, Color> Map => SeverityColors.ToDictionary();

        public Color this[Alert.SeverityLevel index] => Map.ContainsKey(index) ? Map[index] : DefaultSeverityColor;
    }
}