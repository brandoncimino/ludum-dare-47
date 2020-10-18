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
            int index = 0;
            while (index < Assignees.Count) {
                var astronaut = Assignees[index];
                var nonLethal = astronaut.myBody.TakeDamage(0.5f, 1.5f * astronaut.myBody.getDmgVisualTime());

                if (nonLethal) {
                    // the astronaut has survived the damage, and has hence not been removed from the list "Assignees"
                    index += 1;
                }
                else {
                    // report that someone has died
                    Scheduler.Single.ReportEmergency(StationAlertType.Astronaut_Dead_Fire, astronaut);
                    // "index" is not increased because the astronaut dies and has been removed from the list of assignees.
                }
            }

            // fire doesn't cause acceleration
            return 0;
        }
    }
}