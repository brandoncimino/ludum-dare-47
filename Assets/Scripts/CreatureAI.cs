using System.Collections.Generic;

using DefaultNamespace.Text;

using UnityEngine;

namespace DefaultNamespace {
    public class CreatureAI : MonoBehaviour, IAlertReplacements, IMessengers {
        /// <summary>
        /// The name of the <see cref="CreatureAI"/>, for use in alerts.
        /// </summary>
        public string DisplayName => name;

        public virtual void AnnounceArrival() {
            // nothing
        }

        public virtual void AnnounceDeath() {
            // nothing
        }

        public virtual void StartFleeing(bool towardsRight = true) {
            // nothing
        }

        public Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string> {
                {"NAME", DisplayName}
            };
        }

        public virtual void GiveWarning() {
            GiveUpdate();
        }

        public virtual void GiveUpdate() {
            // just do nothing
        }
    }
}