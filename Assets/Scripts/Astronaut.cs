using UnityEngine;

namespace DefaultNamespace {
    public class Astronaut : Creature {
        public ThoughtBubble myThoughtBubble;
        public bool          FleeingRight = true;
        public bool          fleeing;

        public void ChangeThought(Thought newThought) {
            myThoughtBubble.Think(newThought);
        }

        protected override void MovementRule() {
            if (fleeing) {
                // look into the direction you are fleeing
                mySpriteRenderer.flipX = !FleeingRight;

                // move into the direction you are fleeing, a bit faster than usual
                if (FleeingRight) {
                    MoveCounterClockwise(1.3f);
                }
                else {
                    MoveClockwise(1.3f);
                }
            }
            else {
                if (hasArrived) {
                    // if you are there, move a little to show that you are doing something
                    Fidget();
                }
                else {
                    // the creature moves toward its target location
                    MoveTowardTarget();
                }
            }
        }

        public override void Kill() {
            alive = false;

            // no more moving around when you are dead! (for now)
            speedAngle = 0;

            // let's make you a skeleton
            mySpriteRenderer.color = new Color(1, 1, 1);
            // TODO: Sprite for dead creature

            // collapse to the ground, be ejected into space, ...
            // TODO: write movement rule for when killed

            // tell your AI that you are dead
            myBrain.AnnounceDeath();
        }

        public void Scare(float monsterAngle) {
            var fleeRight = false;
            if (monsterAngle - positionAngle > 0) {
                fleeRight = monsterAngle - positionAngle > 180;
            }
            else {
                fleeRight = true;
            }

            myBrain.StartFleeing(fleeRight);

            // reset fidget rotation for when you can stand still again
            fidgetAngle = 0;
        }
    }
}