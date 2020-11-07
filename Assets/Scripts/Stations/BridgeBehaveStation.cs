using System.Collections.Generic;

namespace DefaultNamespace {
    public class BridgeBehaveStation : BehaveStation {
        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                return -1.5f;
            }

            return 0;
        }

        protected override int UpdateDataCount => 1;

        public override Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string>() {
                {"CAPTION1", "Astronauts"},
                {"DATA1", home.noAstronauts.ToString()},
                {"CAPTION2", ""},
                {"DATA2", ""},
                {"ROOM", "BRIDGE"},
                {"TIME", TimeStamp()}
            };
        }
    }
}