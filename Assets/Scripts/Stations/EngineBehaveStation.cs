using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace {
    public class EngineBehaveStation : BehaveStation {
        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                return -2f;
            }

            return 0;
        }

        protected override int UpdateDataCount => 2;

        public override Dictionary<string, string> GetAlertReplacements() {
            return new Dictionary<string, string>() {
                {"CAPTION1", "Rotation-Speed"},
                {"DATA1", home.Speed.ToString()},
                {"CAPTION2", "Wobble-Speed"},
                {"DATA2", home.Wobbler.WobbleSpeed.ToString()},
                {"ROOM", "ENGINE"},
                {"TIME", Time.realtimeSinceStartup.ToString()}
            };
        }
    }
}