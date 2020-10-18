using System;
using System.Collections.Generic;
using System.Linq;

using DefaultNamespace.Text;

using UnityEngine;

namespace DefaultNamespace {
    public class SpaceStation : MonoBehaviour {
        public static SpaceStation Single = null;

        #region Geometric information about the space station

        // geoometric information about the space station
        // space station floor is assumed as a cylinder for simplicity
        public float radius = 2 * 14.1033f;
        public float depth  = 4f;

        #endregion

        #region information about the astronauts living on the space station

        public  GameObject      AstronautPrefab;
        public  GameObject      MonsterPrefab;
        public  List<Astronaut> Astronauts;
        public  List<Monster>   Monsters;
        public  int             noAstronauts        = 2;
        public  int             noLayers            = 7;
        private float           MaxMonsterSpawnTime = 1f;
        public  float           MonsterSpawnTime    = 0f;
        private float           MonsterBreakAngle   = -1f;

        #endregion

        #region information about the station's yaw and Wobbler

        public Wobbler Wobbler;
        public Canvas  Canvas;

        private       float MaxSpeed              = 80f;
        private       float MinSpeed              = 5f;
        private       float MaxStableAcceleration = 5f;
        private       float MinStableDeceleration = -1;
        public        float Speed                 = 10f; // degrees per time step
        public        float AccelerationMod       = 0f;
        public        float ActiveAcceleration    = 0f;
        public        float ActiveDeceleration    = 0f;
        private const float InherentAcceleration  = 2.5f;

        #endregion

        #region information about the station's workstations

        public GameObject             BehaveStationPrefab;
        public GameObject             MisbehaveStationPrefab;
        public List<BehaveStation>    BehaveStations;
        public List<MisbehaveStation> MisbehaveStations;

        #endregion


        private void Awake() {
            if (Single == null) {
                Single = this;
            }
            else {
                Destroy(this.gameObject);
            }
        }

        private void Start() {
            noLayers = noAstronauts + 2;

            // position the behave stations in your list
            var angle = 18f;
            foreach (var station in BehaveStations) {
                station.PlaceDown(angle, 1);
                angle += 72;
            }

            // position the misbehave stations in your list
            angle = 18f;
            foreach (var station in MisbehaveStations) {
                station.PlaceDown(angle, 1);
                angle += 72;
            }

            // spawn astronauts
            for (int i = 0; i < noAstronauts; i++) {
                var newAstronaut = Instantiate(AstronautPrefab).GetComponent<Astronaut>();
                newAstronaut.transform.parent = transform;
                newAstronaut.ChangeLayer(noLayers - i);
                Astronauts.Add(newAstronaut);
            }
        }

        void Update() {
            // update the speed based on current accelerations and decelerations
            ChangeSpeed();

            // rotate
            transform.Rotate(0, Speed * Time.deltaTime, 0);

            // check for monster
            if (MonsterBreakAngle != -1f) {
                MonsterSpawnTime += Time.deltaTime;
                if (MonsterSpawnTime >= MaxMonsterSpawnTime) {
                    MonsterSpawnTime = 0;
                    SpawnMonster(MonsterBreakAngle);
                    MonsterBreakAngle = -1f;
                }
            }
        }

        //[EditorInvocationButton]
        public void Accelerate() {
            AccelerationMod++;
        }

        //[EditorInvocationButton]
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

            // gather what is outside the acceleration bounds
            var newAcc1 = AccelerationMod;
            AccelerationMod = Math.Min(AccelerationMod, MaxStableAcceleration);
            AccelerationMod = Math.Max(AccelerationMod, MinStableDeceleration);
            var excessSpeed = (newAcc1 - AccelerationMod) * Time.deltaTime;

            // Compute new speed
            // acceleration is the derivative of speed, here speed is computed with forward Euler
            var newSpeed1 = Speed + Time.deltaTime * AccelerationMod;

            // acceleration is only converted into speed until maximum or minimum speed is achieved
            // compute how much excess acceleration or deceleration has been caused
            var newSpeed2 = Math.Min(newSpeed1, MaxSpeed);
            Speed       =  Math.Max(MinSpeed, newSpeed2);
            excessSpeed += (newSpeed1 - Speed);
            // var excessAcceleration = excessSpeed / Time.deltaTime;

            // provide excess data to the Wobbler
            Wobbler.Convert2Wobbling(excessSpeed);
        }

        public void SpawnMonster(float angle = 0) {
            // spawn (another??) monster
            var newMonster = Instantiate(MonsterPrefab).GetComponent<Monster>();
            newMonster.transform.parent = transform;
            newMonster.ChangeLayer(noLayers);
            newMonster.positionAngle = angle;
            Monsters.Add(newMonster);
        }

        public void Tragedy(Astronaut deadBody) {
            Astronauts.Remove(deadBody);
            noAstronauts--;

            for (int i = 0; i < noAstronauts; i++) {
                var astronaut = Astronauts[i];
                astronaut.ChangeLayer(noLayers - i);
            }
        }

        public void SpawnAstronaut(float angle = 0) {
            // make new astronaut
            var newAstronaut = Instantiate(AstronautPrefab).GetComponent<Astronaut>();
            newAstronaut.transform.parent = transform;
            newAstronaut.positionAngle    = angle;
            Astronauts.Add(newAstronaut);
            Scheduler.Single.ReportEmergency(StationAlertType.Astronaut_Cloned, newAstronaut.myBrain);

            // distribute evenly
            noAstronauts++;
            for (int i = 0; i < noAstronauts; i++) {
                var astronaut = Astronauts[i];
                astronaut.ChangeLayer(noLayers - i);
            }
        }

        public void JailBreak(float angle = 0) {
            // alert everyone of approaching danger
            foreach (var astronaut in Astronauts.Where(
                astronaut => astronaut.alive && astronaut.Distance2AngleAsAngle(angle) < 20f
            )) {
                // cause to flee
                astronaut.Scare(angle);
            }

            MonsterBreakAngle = angle;
        }
    }
}