namespace DefaultNamespace {
    public class EngineMisbehaveStation : MisbehaveStation {
        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval

            if (Assignees.Count == 0) {
                return 0;
            }

            // misbehaviour in the engine room causes double acceleration
            return 2 * Assignees.Count;
        }
    }
}