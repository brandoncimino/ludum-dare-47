using System;
using System.Collections.Generic;

using Packages.BrandonUtils.Editor;

using UnityEngine;

namespace DefaultNamespace {
    public class SpaceStation : MonoBehaviour {

        // geoometric information about the space station
        // space station floor is assumed as a cylinder for simplicity
        public float radius = 14.1033f;
        public float depth  = 6f;
        
        // information about the astronauts living on the space station
        public GameObject     AstronautPrefab;
        public List<Creature> Astronauts;
        public int            noAstronauts = 5;
        
        // information about the station's spin
        private       float idealSpeed          = 10f;
        public        float Speed               = 10f; // degrees per time step
        private const float AccelerationMod     = 10f;
        private       int   ActiveAccelerations = 0;
        private const float xRotationMod        = 0.3f;
        private       float tumbleDegree        = 0;
        private       bool  tumble2Player       = true;
        
        void Start() {
            // spawn astronauts
            for (int i = 0; i < noAstronauts; i++) {
                var newAstronaut = Instantiate(AstronautPrefab).GetComponent<Creature>();
                newAstronaut.transform.parent = transform;
                newAstronaut.ChangeLayer(i);
                newAstronaut.GiveHome(this);
                Astronauts.Add(newAstronaut);
            }
            
        }

        void Update() {
            
            // compute speed (explicit Euler - exact if the acceleration is piecewise constant)
            Speed       += Time.deltaTime * AccelerationMod * ActiveAccelerations;
            
            // rotate
            transform.Rotate(0, Speed * Time.deltaTime, 0);

            if (tumble2Player) {
                tumbleDegree += Speed * Time.deltaTime;
            }
            
            // transform.Rotate(Speed * (float) Math.Sin(Time.time) * xRotationMod, 0,     0, Space.World);
            // TODO: tumbling, needs some fine-tuning
        }
        
        [EditorInvocationButton]
        public void Accelerate() {
            ActiveAccelerations++;
        }
        
        [EditorInvocationButton]
        public void Decelerate() {
            ActiveAccelerations--;
        }
    }
}
