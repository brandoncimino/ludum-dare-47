using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.Text {
    /// <summary>
    /// This class contains the unformatted alert messages as well as the methods that format them.
    /// </summary>
    public class StationLog {
        public Dictionary<StationAlertType, List<string>> Alerts;

        /// <summary>
        /// Formats <paramref name="key"/> according to the expectations of our <see cref="Alerts"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string FormatKey(string key) {
            return $"${key}";
        }

        /// <summary>
        /// Formats <paramref name="alert"/> by finding the instances of <see cref="Dictionary{TKey,TValue}.Keys"/> from <paramref name="replacements"/> and <see cref="string.Replace(string,string)"/>-ing them with their corresponding <see cref="Dictionary{TKey,TValue}.Values"/>.
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Similar to <see cref="FormatAlert(string,System.Collections.Generic.Dictionary{string,string})"/>, but <see cref="Enumerable.Concat{TSource}"/>s <paramref name="replacements"/> with <see cref="GetReplacements"/>(<paramref name="replacementSources"/>).
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="replacements"></param>
        /// <param name="replacementSources"></param>
        /// <returns></returns>
        public static string FormatAlert(
            string alert,
            Dictionary<string, string> replacements,
            params IAlertReplacements[] replacementSources
        ) {
            return FormatAlert(
                alert,
                (Dictionary<string, string>) replacements.Concat(GetReplacements(replacementSources))
            );
        }

        /// <summary>
        /// Similar to <see cref="FormatAlert(string,System.Collections.Generic.Dictionary{string,string})"/>, using <see cref="GetReplacements"/>(<paramref name="replacementSources"/>) as the replacements.
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="replacementSources"></param>
        /// <returns></returns>
        public static string FormatAlert(string alert, params IAlertReplacements[] replacementSources) {
            return FormatAlert(alert, GetReplacements(replacementSources));
        }

        /// <summary>
        /// Calls <see cref="IAlertReplacements.GetAlertReplacements"/> against an <see cref="IEnumerable{T}"/> of <see cref="IAlertReplacements"/>.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetReplacements(IEnumerable<IAlertReplacements> inputs) {
            var result = new Dictionary<string, string>();

            foreach (var input in inputs) {
                result = result.Concat(input.GetAlertReplacements()).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return result;
        }
    }
}