using UnityEngine;

namespace DefaultNamespace {
    public class CreatureAI : MonoBehaviour {
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