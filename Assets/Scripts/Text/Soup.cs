using System.Collections.Generic;
using System.Linq;

using Packages.BrandonUtils.Runtime.Collections;

namespace DefaultNamespace.Text {
    /// <summary>
    /// Contains information about a soup sourced from Wikipedia's <a href="https://en.wikipedia.org/wiki/List_of_soups">List of Soups</a>.
    /// </summary>
    public class Soup : IAlertReplacements {
        public string Name;
        public string Origin;
        public string Type;
        public string Details;

        public override string ToString() {
            var extras = new List<string> {Origin, Type}.Where(it => !string.IsNullOrWhiteSpace(it)).JoinString(", ");
            return $"{Name} ({extras})";
        }

        public Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string> {
                {"SOUP", ToString()}
            };
        }
    }
}