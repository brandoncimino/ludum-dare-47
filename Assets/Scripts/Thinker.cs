using UnityEngine;

namespace DefaultNamespace {
    /// <summary>
    /// This is an "interface" that holds a <see cref="CurrentThought"/>.
    /// </summary>
    /// <remarks>
    /// In retrospect, most of the logic inside of <see cref="ThoughtBubble"/> should have been inside of <see cref="Thinker"/> - oh well.
    /// </remarks>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Thinker : MonoBehaviour {
        public Thought CurrentThought;
    }
}