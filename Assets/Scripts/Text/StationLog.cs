using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.Text {
    public class StationLog {
        public Dictionary<StationAlertType, List<string>> Alerts;

        private static string FormatKey(string key) {
            return $"${key}";
        }

        public static string FormatAlert(string alert, Dictionary<string, string> replacements = null) {
            if (replacements != null) {
                return replacements.Aggregate(
                    alert,
                    (current, pair) => current.Replace(FormatKey(pair.Key), pair.Value)
                );
            }
            else {
                return alert;
            }
        }

        public static string FormatAlert(string alert, CreatureAI creature) {
            return creature == null ? alert : FormatAlert(alert, GetReplacements(creature));
        }

        private static Dictionary<string, string> GetReplacements(CreatureAI creature) {
            return new Dictionary<string, string> {
                {"NAME", creature.DisplayName}
            };
        }
    }
}