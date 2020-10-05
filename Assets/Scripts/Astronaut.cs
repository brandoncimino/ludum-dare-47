namespace DefaultNamespace {
    public class Astronaut : Creature {
        public ThoughtBubble myThoughtBubble;
        public AstroAI       myBrain;
        public bool          FleeingRight = true;

        public void ChangeThought(Thought newThought) {
            myThoughtBubble.Think(newThought);
        }

        protected override void MovementRule() {
            if (myBrain.myStats.myState == AstroStats.AIStates.Fleeing) {
                if (FleeingRight) {
                    MoveClockwise();
                }
                else {
                    MoveCounterClockwise();
                }
            }
            else {
                // the creature moves toward its target location
                MoveTowardTarget();
            }
        }
    }
}