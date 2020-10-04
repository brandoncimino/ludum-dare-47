using System.Collections.Generic;

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
            // rotate
        }
        
    }
}
