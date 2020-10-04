using Packages.BrandonUtils.Editor;
using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

namespace DefaultNamespace {
    public class ThoughtBubble : MonoBehaviour {
        public Sprite BoredomSprite;
        public Sprite MischiefSprite;
        public Sprite PyromaniaSprite;
        public Sprite AlarmSprite;
        public Sprite MemesSprite;

        public       SpriteRenderer ThoughtRenderer;
        public       SpriteRenderer BubbleRenderer;
        public       Thinker        Thinker;
        public const float          ThinkDuration_Default = 3;
        public       float          ThinkDuration         = 99;
        public       bool           Thinking => ThinkDuration > 0;

        private void Awake() {
            SnapToThinker();
        }

        public Sprite GetThoughtSprite(Thought thought) {
            switch (thought) {
                case Thought.Boredom:
                    return BoredomSprite;
                case Thought.Mischief:
                    return MischiefSprite;
                case Thought.Pyromania:
                    return PyromaniaSprite;
                case Thought.Alarm:
                    return AlarmSprite;
                case Thought.Memes:
                    return MemesSprite;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(thought), thought);
            }
        }

        public void Think(Thought thought, float duration = ThinkDuration_Default) {
            Thinker.CurrentThought = thought;
            ThinkDuration          = duration;
            UpdateThoughtBubbleSprites();
        }

        private void Update() {
            ThinkDuration = Mathf.Max(0, ThinkDuration - Time.deltaTime);

            UpdateThoughtBubbleSprites();
        }

        private void UpdateThoughtBubbleSprites() {
            ThoughtRenderer.sprite  = GetThoughtSprite(Thinker.CurrentThought);
            BubbleRenderer.enabled  = Thinking;
            ThoughtRenderer.enabled = BubbleRenderer.enabled;
        }

        [EditorInvocationButton]
        public void NextThought() {
            Think(Thinker.CurrentThought.Next());
        }

        [EditorInvocationButton]
        private void SnapToThinker() {
            var thinkerRenderer = Thinker.GetComponent<SpriteRenderer>();
            /*
             * This stuff was a massively over-complicated way to position the thought bubble that only worked if the astronaut never rotated.
             */
            // var bounds          = thinkerRenderer.bounds;
            // LogUtils.Log($"center = {bounds.center}",
            //              "extends = "+bounds.extents,
            //              "min = "+bounds.min,
            //              "max = "+bounds.max,
            //              "size = "+bounds.size);
            // transform.position = new Vector3(
            //     bounds.center.x,
            //     bounds.center.y + bounds.extents.y,
            //     Thinker.transform.position.z
            // );
            transform.localPosition = Vector3.up;
        }
    }
}