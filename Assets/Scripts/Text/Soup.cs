using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace.Text {
    /// <summary>
    /// Contains information about a <a href="https://en.wikipedia.org/wiki/Soup">soup</a> sourced from Wikipedia's <a href="https://en.wikipedia.org/wiki/List_of_soups">List of Soups</a>.
    /// </summary>
    public class Soup : IAlertReplacements {
        public string Name;
        public string Origin;
        public string Type;
        public string Details;

        public override string ToString() {
            //grab all of the "extra" info we want that _isn't null or whitespace_
            var extras = new List<string> {Origin, Type}.Where(it => !string.IsNullOrWhiteSpace(it));

            //if we have any extras, append them to the name in parentheses; otherwise, just return the name
            return extras.Any() ? $"{Name} ({extras})" : Name;
        }

        public Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string> {
                {"SOUP", ToString()}
            };
        }
    }
}