namespace DefaultNamespace {
    public class BridgeBehaveStation : BehaveStation {
        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                return -1.5f;
            }

            return 0;
        }
    }
}