namespace DefaultNamespace {
    public class LabBehaveStation : BehaveStation {
        private float IncarnationProcess = 0;
        private float IncarnationTime    = 30f;
        public  bool  MonsterInStorage   = true;

        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                // do "good" science
                return ScienceUp(timePassed);
            }

            return 0;
        }

        private float ScienceUp(float timePassed) {
            IncarnationProcess += timePassed;

            if (!MonsterInStorage && IncarnationProcess >= 0.2f * IncarnationTime) {
                MonsterInStorage = true;
            }

            if (IncarnationProcess >= IncarnationTime) {
                home.SpawnAstronaut(PositionAngle);
                IncarnationProcess = 0;
                MonsterInStorage   = false;
            }

            return 0;
        }
    }
}