using System;
using System.Collections.Generic;

using Packages.BrandonUtils.Editor;

using UnityEngine;

namespace DefaultNamespace {
    public class SpaceStation : MonoBehaviour {
        #region Geometric information about the space station

        // geoometric information about the space station
        // space station floor is assumed as a cylinder for simplicity
        public float radius = 2*14.1033f;
        public float depth  = 4f;

        #endregion

        #region information about the astronauts living on the space station

        public GameObject      AstronautPrefab;
        public GameObject      MonsterPrefab;
        public List<Astronaut> Astronauts;
        public List<Monster>   Monsters;
        public int             noAstronauts = 2;
        public int             noLayers     = 7;

        #endregion

        #region information about the station's yaw and Wobbler

        public Wobbler Wobbler;

        private       float MaxSpeed             = 50f;
        private       float MinSpeed             = 5f;
        public        float Speed                = 10f; // degrees per time step
        public        float AccelerationMod      = 0f;
        public        float ActiveAcceleration   = 0f;
        public        float ActiveDeceleration   = 0f;
        private const float InherentAcceleration = 2.5f; // TODO: noAstronauts / 2.0f
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
                var newAstronaut = Instantiate(AstronautPrefab).GetComponent<Astronaut>();
                newAstronaut.transform.parent = transform;
                newAstronaut.ChangeLayer(noLayers - i);
                newAstronaut.GiveHome(this);
                Astronauts.Add(newAstronaut);
            }
            
            // spawn activity stations
            var angle = 18f;
            foreach (ActivityRoom doorSign in Enum.GetValues(typeof(ActivityRoom))) {
                
                // add Behave Stations
                var newBehaveStation = Instantiate(BehaveStationPrefab).GetComponent<BehaveStation>();
                newBehaveStation.transform.parent = transform;
                newBehaveStation.PlaceDown(angle, 1, this, doorSign);
                
                // add Misbehave Stations
                var newMisbehaveStation = Instantiate(MisbehaveStationPrefab).GetComponent<MisbehaveStation>();
                newMisbehaveStation.transform.parent = transform;
                newMisbehaveStation.PlaceDown(angle, 1, this, doorSign);

                // inform about their twins
                newBehaveStation.behaveTwin    = newMisbehaveStation;
                newMisbehaveStation.behaveTwin = newBehaveStation;
                
                // add to station list
                // TODO: write setter routine, make twinStations private property
                BehaveStations.Add(newBehaveStation);
                MisbehaveStations.Add(newMisbehaveStation);

                // change where to position the next station center
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
                ActiveDeceleration += station.DetermineConsequences(Time.deltaTime);
            }
            
            foreach (var station in MisbehaveStations) {
                ActiveAcceleration += station.DetermineConsequences(Time.deltaTime);
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
            // var excessAcceleration = excessSpeed / Time.deltaTime;
            
            // provide excess data to the Wobbler
            Wobbler.Convert2Wobbling(excessSpeed);
            
        }

        public void SpawnMonster(float angle = 0) {
            
            // spawn (another??) monster
            var newMonster = Instantiate(MonsterPrefab).GetComponent<Monster>();
            newMonster.transform.parent = transform;
            newMonster.ChangeLayer(noLayers);
            newMonster.GiveHome(this);
            newMonster.positionAngle = angle;
            Monsters.Add(newMonster);
            
        }
    }
}
