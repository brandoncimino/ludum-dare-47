using System.Collections.Generic;
using System.Linq;

using DefaultNamespace.Text;

using UnityEngine;

namespace DefaultNamespace {
    public class KitchenBehaveStation : BehaveStation {
        public override float DetermineConsequences(float timePassed) {
            if (currentState == BehaveStationStates.Occupied) {
                if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                    // if the station next to you is broken, repair it first
                    RepairTwin(timePassed);

                    // while repairing, we cannot put active work into deceleration
                    return 0;
                }

                return -1f;
            }

            return 0f;
        }

        protected override int UpdateDataCount => 1;

        public override Dictionary<string, string> GetAlertReplacements() {
            var health = home.Astronauts.Sum(astronaut => astronaut.currentHitPoints) / home.noAstronauts;

            // create the dictionary with the general stuff
            var stdDictionary = new Dictionary<string, string>() {
                {"CAPTION1", "Average Astronaut health"},
                {"DATA1", health.ToString()},
                {"CAPTION2", ""},
                {"DATA2", ""},
                {"ROOM", "KITCHEN"},
                {"TIME", Time.realtimeSinceStartup.ToString()}
            };

            // also include the special (key, value) pairings for the soup and its description
            StationLogger.SoupOfTheDay.GetAlertReplacements()
                         .ToList()
                         .ForEach(pair => stdDictionary.Add(pair.Key, pair.Value));
            return stdDictionary;
        }
    }
}