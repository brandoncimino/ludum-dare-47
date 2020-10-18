namespace DefaultNamespace {
    public class BridgeMisbehaveStation : MisbehaveStation {
        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval

            if (Assignees.Count == 0) {
                return 0;
            }

            // misbehaviour on the bridge causes double acceleration
            return 2 * Assignees.Count;
        }
    }
}