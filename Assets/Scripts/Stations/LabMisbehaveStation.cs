namespace DefaultNamespace {
    public class LabMisbehaveStation : MisbehaveStation {
        public new LabBehaveStation behaveTwinLab;

        private void Start() {
            remainingBreakTime = maxTimeToBreak;
            AstroForeman.Single.Register(this);
            base.behaveTwin = behaveTwinLab;
        }

        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval

            if (Assignees.Count == 0) {
                return 0;
            }

            // misbehaviour in the lab frees the Monster
            return FreeMonster(timePassed);
        }

        private float FreeMonster(float timePassed) {
            // returns acceleration caused in the process

            // damage the incarnation tube
            remainingBreakTime -= timePassed * Assignees.Count;
            currentState       =  MisbehaveStationStates.Damaged;

            // check if broken
            if (behaveTwinLab.MonsterInStorage && remainingBreakTime <= 0) {
                // consider this station broken
                currentState       = MisbehaveStationStates.Broken;
                remainingBreakTime = 0;
                home.JailBreak(PositionAngle);
                behaveTwinLab.MonsterInStorage = false;
            }

            // freeing a monster doesn't cause acceleration
            return 0;
        }

        protected override bool Reason2Warn(float threshold = 0.7f) {
            // a reason to warn is if the incarnation tube's hp have been reduced by at least (threshold * 100)%
            return remainingBreakTime / maxTimeToBreak <= threshold;
        }
    }
}