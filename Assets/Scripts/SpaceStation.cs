using System;
using System.Collections.Generic;

using Packages.BrandonUtils.Editor;

using UnityEngine;

namespace DefaultNamespace {
    public class SpaceStation : MonoBehaviour {
        #region Geometric information about the space station

        // geoometric information about the space station
        // space station floor is assumed as a cylinder for simplicity
        public float radius = 14.1033f;
        public float depth  = 6f;

        #endregion

        #region information about the astronauts living on the space station

        public GameObject     AstronautPrefab;
        public List<Creature> Astronauts;
        public int            noAstronauts = 2;
        public int            noLayers     = 7;

        #endregion

        #region information about the station's yaw and Wobbler

        public Wobbler Wobbler;

        private       float MaxSpeed             = 50f;
        private       float MinSpeed             = 5f;
        public        float Speed                = 10f; // degrees per time step
        public        float AccelerationMod      = 0f;
        public        float ActiveAcceleration   = 0f;
        public        float ActiveDeceleration   = 0f;
        private const float InherentAcceleration = 1f; // TODO: noAstronauts / 2.0f
        #endregion
        
        #region information about the station's workstations
        public GameObject             BehaveStationPrefab;
        public GameObject             MisbehaveStationPrefab;
        public List<BehaveStation>    BehaveStations;
        public List<MisbehaveStation> MisbehaveStations;
        #endregion

        void Start() {
            
            // spawn astronauts
            for (int i = 0; i < noAstronauts; i++) {
                var newAstronaut = Instantiate(AstronautPrefab).GetComponent<Creature>();
                newAstronaut.transform.parent = transform;
                newAstronaut.ChangeLayer(noLayers - i);
                newAstronaut.GiveHome(this);
                Astronauts.Add(newAstronaut);
            }
            
            // spawn activity stations
            var angle = 0f;
            foreach (ActivityRoom doorSign in Enum.GetValues(typeof(ActivityRoom))) {
                
                // add Behave Stations
                var newBehaveStation = Instantiate(BehaveStationPrefab).GetComponent<BehaveStation>();
                newBehaveStation.transform.parent = transform;
                newBehaveStation.PlaceDown(angle, 1, this, doorSign);
                BehaveStations.Add(newBehaveStation);
                
                // add Misbehave Stations
                var newMisbehaveStation = Instantiate(MisbehaveStationPrefab).GetComponent<MisbehaveStation>();
                newMisbehaveStation.transform.parent = transform;
                newMisbehaveStation.PlaceDown(angle, 1, this, doorSign);
                MisbehaveStations.Add(newMisbehaveStation);

                angle += 72;
            }
            
        }

        void Update() {
            
            // update the speed based on current accelerations and decelerations
            ChangeSpeed();
            
            // rotate
            transform.Rotate(0, Speed * Time.deltaTime, 0);
        }

        [EditorInvocationButton]
        public void Accelerate() {
            AccelerationMod++;
        }

        [EditorInvocationButton]
        public void Decelerate() {
            AccelerationMod--;
        }

        private void GatherAcceleratons() {

            ActiveAcceleration = 0f;
            ActiveDeceleration = 0f;
            
            foreach (var station in BehaveStations) {
                ActiveDeceleration += station.SpeedInfluence(Time.deltaTime);
            }
            
            foreach (var station in MisbehaveStations) {
                ActiveAcceleration += station.SpeedInfluence(Time.deltaTime);
            }

            AccelerationMod = ActiveAcceleration + ActiveDeceleration + InherentAcceleration;
        }

        private void ChangeSpeed() {
            
            // Combine all sources for speed change
            GatherAcceleratons();
            
            // Compute new speed
            // acceleration is the derivative of speed, here speed is computed with forward Euler
            var newSpeed1 = Speed + Time.deltaTime * AccelerationMod;
            
            // acceleration is only converted into speed until maximum or minimum speed is achieved
            // compute how much excess acceleration or deceleration has been caused
            var newSpeed2 = Math.Min(newSpeed1, MaxSpeed);
            Speed = Math.Max(MinSpeed, newSpeed2);
            var excessSpeed        = newSpeed1 - Speed;
            var excessAcceleration = excessSpeed / Time.deltaTime;
            
            // provide excess data to the Wobbler
            Wobbler.Convert2Wobbling(ActiveAcceleration, ActiveDeceleration, excessAcceleration);
            
        }
    }
}
