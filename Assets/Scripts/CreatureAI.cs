using UnityEngine;

namespace DefaultNamespace {
    public class CreatureAI : MonoBehaviour {
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
    }
}