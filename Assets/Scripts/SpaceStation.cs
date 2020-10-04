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
        private       float Speed              = 0.1f; // degrees per time step
        private const float Acceleration       = 1f;
        private       int   ActiveAccelerations = 0;
        
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
            Speed += Time.deltaTime * Acceleration * ActiveAccelerations;
            
            // rotate
            transform.Rotate(0, Speed, 0);
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
