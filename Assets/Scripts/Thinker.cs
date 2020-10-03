using UnityEngine;

namespace DefaultNamespace {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Thinker : MonoBehaviour {
        public Thought CurrentThought;
    }
}