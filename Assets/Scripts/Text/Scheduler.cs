using System;
using System.Collections.Generic;

using Packages.BrandonUtils.Runtime.Collections;

using UnityEngine;

using Random = System.Random;

namespace DefaultNamespace.Text {
    public class Scheduler : MonoBehaviour {
        public static Scheduler     Single;
        private       StationLogger StationLogger = StationLogger.Single;
        private       Random        rand          = new Random();

        #region time management

        // base and emergency
        private float MinWaitTime = 0.3f;
        private float WaitCounter = 0f;

        // warnings
        public  List<IMessengers> myWarnings     = new List<IMessengers>();
        private float             WarnTimeMean   = 3.0f;
        private float             WarnTimeStdDev = 1.5f;
        private float             WarnTimeMax    = 3f;
        private float             WarnCounter    = 0f;
        public  float             WarnThreshold  = 1f;

        // scheduled update messages
        private float UpdateCounter   = 0f;
        private float UpdateThreshold = 4f;
        private int   UpdateIndex     = 0;

        // info messages
        private float InfoTimeMean   = 4f;
        private float InfoTimeStdDev = 1.5f;
        private float InfoTimeMax    = 10f;
        private float InfoCounter    = 0f;
        public  float InfoThreshold  = 2f;

        #endregion

        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        private void Update() {
            // update time counters
            var timePassed = Time.deltaTime;
            WaitCounter   += timePassed;
            WarnCounter   += timePassed;
            UpdateCounter += timePassed;
            InfoCounter   += timePassed;

            // only print alerts if the minimum time has passed
            if (WaitCounter < MinWaitTime) {
                return;
            }

            // warnings come first
            if (myWarnings.Count > 0 && WarnCounter >= WarnThreshold) {
                // give out the warning
                var warned = GiveWarning();
                if (warned) {
                    // determine new warning threshold and reset timer
                    WarnThreshold = Math.Min(GaussSample(WarnTimeMean, WarnTimeStdDev), WarnTimeMax);
                    WarnCounter   = 0;
                    WaitCounter   = 0;
                    return;
                }
            }

            // prefer scheduled updates over disturbing the astronauts
            if (UpdateCounter >= UpdateThreshold) {
                // choose which station should give the update
                var station = SpaceStation.Single.BehaveStations[UpdateIndex];
                UpdateIndex = (UpdateIndex + 1) % SpaceStation.Single.BehaveStations.Count;

                // ask for the alert
                station.GiveUpdate();

                // reset timer
                UpdateCounter = 0;
                WaitCounter   = 0;
                return;
            }

            // now ask astronauts if they have something to say
            if (InfoCounter >= InfoThreshold) {
                // choose which station should give the update
                var astronaut = SpaceStation.Single.Astronauts.Random().myBrain;

                // ask for the alert
                astronaut.GiveUpdate();

                // reset timer and find new threshold
                InfoCounter   = 0;
                WaitCounter   = 0;
                InfoThreshold = Math.Min(GaussSample(InfoTimeMean, InfoTimeStdDev), InfoTimeMax);
            }
        }

        public void ReportEmergency(StationAlertType alertType, params IAlertReplacements[] replacementSources) {
            StationLogger.Alert(alertType, Alert.SeverityLevel.Emergency, replacementSources);
            WaitCounter = 0f;
        }

        public void ReportWarning(IMessengers messenger) {
            myWarnings.Add(messenger);
        }

        /// <summary>
        /// Takes a sample from a Gaussian distribution with specified mean and standard deviation.
        /// Apparently C# doesn't have this build in, only uniformly distributed random numbers.
        /// This implementation uses the Box-Muller transform, see
        /// https://en.wikipedia.org/wiki/Box–Muller_transform
        /// Credit goes to this stackoverflow thread:
        /// https://stackoverflow.com/questions/218060/random-gaussian-variables
        /// </summary>
        /// <param name="mean">mean</param>
        /// <param name="stdDev">standard deviation = sqrt(variance)</param>
        /// <returns></returns>
        private float GaussSample(float mean, float stdDev) {
            var u1            = 1.0 - rand.NextDouble();
            var u2            = 1.0 - rand.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return (float) (mean + stdDev * randStdNormal);
        }

        /// <summary>
        /// This function selects a random messenger who has before reported that they wanted to warn, and gives them the change to do so. If the messenger no longer sees a reason to warn, another messenger is chosen, until a warning has been raised or all messengers have been cleared.
        /// </summary>
        /// <returns>returns (bool) whether or not a warning has been raised</returns>
        private bool GiveWarning() {
            var warned = false;
            while (!warned && myWarnings.Count > 0) {
                // we are nice and only reset the warning timer if we have indeed warned

                // select messenger, for now randomly
                // TODO: base choice on importance
                var messenger = myWarnings.Random();

                // deliver message
                warned = messenger.GiveWarning();

                // remove messenger
                myWarnings.Remove(messenger);
            }

            return warned;
        }
    }
}