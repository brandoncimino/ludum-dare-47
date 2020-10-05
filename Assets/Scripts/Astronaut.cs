namespace DefaultNamespace {
    public class Astronaut : Creature {
        public Thinker myThinkingPart;
        public void ChangeThought(Thought newThought) {
            myThinkingPart.CurrentThought = newThought;
        }
    }
}