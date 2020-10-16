using System;
using System.Collections.Generic;

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
        private List<IMessengers> myWarnings     = new List<IMessengers>();
        private float             WarnTimeMean   = 1.5f;
        private float             WarnTimeStdDev = 0.5f;
        private float             WarnTimeMax    = 3f;
        private float             WarnCounter    = 0f;
        private float             WarnThreshold  = 0f;

        // scheduled update messages
        private float UpdateCounter   = 0f;
        private float UpdateThreshold = 4f;

        // info messages
        private float MeanInfoTime  = 2.5f;
        private float MaxInfoTime   = 6f;
        private float InfoCounter   = 0f;
        private float InfoThreshold = 0f;

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

            // prefer warnings
            if (myWarnings.Count > 0 && WarnCounter >= WarnThreshold) {
                // select messenger
                var messenger = myWarnings[0];

                // deliver message

                // remove messenger
                myWarnings.Remove(messenger);

                // determine new warning threshold
                WarnThreshold = Math.Min(GaussSample(WarnTimeMean, WarnTimeStdDev), WarnTimeMax);
                return;
            }
        }

        public void ReportEmergency(StationAlertType alertType, params IAlertReplacements[] replacementSources) {
            StationLogger.Alert(alertType, replacementSources);
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
        public float GaussSample(float mean, float stdDev) {
            var u1            = 1.0 - rand.NextDouble();
            var u2            = 1.0 - rand.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1) * Math.Sin(2.0 * Math.PI * u2));
            return (float) (mean + stdDev * randStdNormal);
        }
    }
}