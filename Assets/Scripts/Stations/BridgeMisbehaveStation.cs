using UnityEngine;

namespace DefaultNamespace {
    public class BridgeMisbehaveStation : MisbehaveStation {
        public Sprite BridgeSprite;
        public Sprite BridgeSpriteCat;

        public override float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval
            // misbehaviour on the bridge causes double acceleration
            return 2f * Assignees.Count;
        }

        protected override void PlaceDownCorrection() {
            // the big screen on the bridge is behind the control panel
            PositionLayer -= 1;
        }

        protected override bool Arrive_individual(AstroAI astronaut) {
            // start showing cat videos
            mySpriteRenderer.sprite = BridgeSpriteCat;
            return true;
        }

        public override bool Leave(AstroAI astronaut) {
            if (Assignees.Contains(astronaut)) {
                mySpriteRenderer.sprite = BridgeSprite;
                Assignees.Remove(astronaut);
                return true;
            }

            return false;
        }

        protected override bool Leave_individual(AstroAI astronaut) {
            if (Assignees.Count == 0) {
                // stop showing cat videos
                mySpriteRenderer.sprite = BridgeSprite;
            }

            return true;
        }
    }
}