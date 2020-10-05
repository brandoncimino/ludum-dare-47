using System.Collections.Generic;

using UnityEngine.PlayerLoop;

namespace DefaultNamespace {
    public class HealthManager {
        public Wobbler DasWobble;
        public List<ActivityStation> AllStations = new List<ActivityStation>();

        void FixedUpdate() {
            foreach (var thisStation in AllStations) {
                
            }
        }
    }
}