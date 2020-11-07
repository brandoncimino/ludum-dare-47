using System.Collections.Generic;

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

        protected override int UpdateDataCount => 2;

        public override Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string>() {
                {"CAPTION1", "Research Progression"},
                {"DATA1", string.Join("", ((int) (IncarnationProcess / IncarnationTime) * 100).ToString(), "%")},
                {"CAPTION2", "Tube Quality"}, {
                    "DATA2",
                    string.Join(
                        "",
                        ((int) (behaveTwin.remainingBreakTime / behaveTwin.maxTimeToBreak) * 100).ToString(),
                        "%"
                    )
                },
                {"ROOM", "LAB"},
                {"TIME", TimeStamp()}
            };
        }
    }
}