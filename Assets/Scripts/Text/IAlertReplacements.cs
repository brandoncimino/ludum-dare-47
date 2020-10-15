using System.Collections.Generic;

namespace DefaultNamespace.Text {
    /// <summary>
    /// This interface generates "replacements" for use when formatting <see cref="StationLog.Alerts"/> via methods like <see cref="StationLog.FormatAlert(string,System.Collections.Generic.Dictionary{string,string})"/>.
    /// </summary>
    public interface IAlertReplacements {
        /// <summary>
        /// Returns a <see cref="Dictionary{TKey,TValue}"/> to be used as the replacements in <see cref="StationLog.FormatAlert(string,System.Collections.Generic.Dictionary{string,string})"/>.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetAlertReplacements();
    }
}