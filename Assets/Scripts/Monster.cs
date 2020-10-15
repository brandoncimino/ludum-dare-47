using UnityEngine;

namespace DefaultNamespace {
    public class Monster : Creature {
        private bool  MovingRight   = true;
        private float FleeAuraAngle = 30f;
        private float DmgAuraAngle  = 15f;

        public override void OnMouseDown() {
            if (Input.GetMouseButtonDown(0)) {
                TakeDamage(0.5f, maxDmgVisualizationTime);
            }
        }

        protected override void MovementRule() {
            if (hasArrived) {
                hasArrived  = false;
                targetAngle = (targetAngle + rando.Next(-80, 80) + 360) % 360;
            }

            MoveTowardTarget();
        }

        protected override void IndividualUpdate() {
            // do damage and cause astronauts to flee

            if (!alive) {
                // only cause fear and damage if alive
                return;
            }

            foreach (var astronaut in home.Astronauts) {
                // only scare astronauts that aren't fleeing already
                if (astronaut.alive && !astronaut.fleeing) {
                    // compute distance to them
                    var distance = Distance2AngleAsAngle(astronaut.positionAngle);

                    // check what happens
                    if (distance < FleeAuraAngle) {
                        // cause to flee
                        astronaut.Scare(positionAngle);

                        if (distance < DmgAuraAngle) {
                            // deliver damage
                            astronaut.TakeDamage(6.0f);
                        }
                    }
                }
            }
        }

        public override void Kill() {
            home.Monsters.Remove(this);
            Destroy(this.gameObject);
        }
    }
}