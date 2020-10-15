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
                if (FleeingRight) {
                    MoveClockwise();
                }
                else {
                    MoveCounterClockwise();
                }
            }
            else {
                // the creature moves toward its target location
                if (!hasArrived) {
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

        public void Scare(bool monsterMovingRight) {
            myBrain.StartFleeing(monsterMovingRight);
        }
    }
}