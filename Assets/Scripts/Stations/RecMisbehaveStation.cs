namespace DefaultNamespace {
    public class RecMisbehaveStation : MisbehaveStation {
        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval

            if (Assignees.Count == 0) {
                return 0;
            }

            // misbehaviour in the rec room causes damage to the window
            return BreakWindow(timePassed);
        }

        private float BreakWindow(float timePassed) {
            // returns acceleration caused in the process

            // damage the window
            remainingBreakTime -= timePassed * Assignees.Count;
            currentState       =  MisbehaveStationStates.Damaged;

            // check if broken
            if (remainingBreakTime <= 0) {
                currentState       = MisbehaveStationStates.Broken;
                remainingBreakTime = 0;

                // TODO: kill all astronauts (or let them die slowly)

                // TODO: tell the game that it's over (unless we decide a completely broken window is no reason to end it)

                return 0;
            }

            // an intact window doesn't cause acceleration
            // TODO: maybe a percentage?
            return 0;
        }

        protected override void PlaceDownCorrection() {
            // make the window big, centered, and behind the treadmill
            OffsetDistance       =  0;
            transform.localScale *= 2;
            PositionLayer        -= 1;
        }
    }
}