using System.Collections.Generic;
using System.Linq;

using DefaultNamespace.Text;

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
            var health = 0;
            if (home.noAstronauts > 0) {
                health = (int) (home.Astronauts.Sum(astronaut => astronaut.getLifePercentage()) * 100 /
                                (home.noAstronauts));
            }

            // create the dictionary with the general stuff
            var stdDictionary = new Dictionary<string, string>() {
                {"CAPTION1", "Average Astronaut health"},
                {"DATA1", string.Join("", health.ToString(), "%")},
                {"CAPTION2", ""},
                {"DATA2", ""},
                {"ROOM", "KITCHEN"},
                {"TIME", TimeStamp()}
            };

            // also include the special (key, value) pairings for the soup and its description
            StationLogger.SoupOfTheDay.GetAlertReplacements()
                         .ToList()
                         .ForEach(pair => stdDictionary.Add(pair.Key, pair.Value));
            return stdDictionary;
        }
    }
}