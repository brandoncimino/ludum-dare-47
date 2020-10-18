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

            // fire in the kitchen harms a all astronauts at the arson spot
            foreach (var astronaut in Assignees) {
                var nonLethal = astronaut.myBody.TakeDamage(0.5f, 1.5f * astronaut.myBody.getDmgVisualTime());

                if (!nonLethal) {
                    // report that someone has died
                    Scheduler.Single.ReportEmergency(StationAlertType.Astronaut_Dead_Fire, astronaut);
                }
            }

            // fire doesn't cause acceleration
            return 0;
        }
    }
}