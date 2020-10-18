namespace DefaultNamespace {
    public class RecBehaveStation : BehaveStation {
        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                return -1f;
            }

            return 0;
        }

        protected override void PlaceDownCorrection() {
            // the sprite for the treadmill is a bit small so we scale it up
            transform.localScale *= 1.5f;
        }
    }
}