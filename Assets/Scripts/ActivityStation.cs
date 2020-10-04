using System;
using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace {
    public class ActivityStation : MonoBehaviour {
        public List<AstroAI> Assignees;

        private void Awake() {
            //Let foreman know you exist
            
        }

        public virtual bool CanRegister() {
            return true;
        }

        public bool Occuppied => Assignees.Count != 0;
        
        /*public float GetNewTargetAngle() {
            //Either the new Station has the angle baked in, or use a function to find the angle
            return AstronautVisuals.SetTarget(gameObject.transform.position);
        }
        */
    }
}