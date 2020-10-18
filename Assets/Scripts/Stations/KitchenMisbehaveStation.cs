using DefaultNamespace.Text;

namespace DefaultNamespace {
    public class KitchenMisbehaveStation : MisbehaveStation {
        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval

            if (Assignees.Count == 0) {
                return 0;
            }

            // misbehaviour in the kitchen sets fire which hurts the astronauts
            return SetFire(timePassed);
        }

        private float SetFire(float timePassed) {
            // returns acceleration caused in the process

            // fire in the kitchen harms a random astronaut at the arson spot
            // the random choice prevents an error where an astronaut dies from fire damage, gets unassigned from the
            // arson spot, which changes the Assignees list and hence breaks the for-loop over its items
            if (Random.Next(0, 100) == 0) {
                var astronaut = Assignees[Random.Next(0, Assignees.Count)];
                var nonLethal = astronaut.myBody.TakeDamage(0.5f, 1.5f * astronaut.myBody.getDmgVisualTime());

                if (!nonLethal) {
                    Scheduler.Single.ReportEmergency(StationAlertType.Astronaut_Dead_Fire, astronaut);
                }
            }

            // fire doesn't cause acceleration
            return 0;
        }
    }
}