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

        public override bool Arrive(AstroAI astronaut) {
            if (!Assignees.Contains(astronaut)) {
                // start showing cat videos
                mySpriteRenderer.sprite = BridgeSpriteCat;

                Assignees.Add(astronaut);

                // send an alert message for the arrival at this station.
                //TODO: This would ideally use an event system, either with "real" events or UnityEvents, but that might require more refactoring than is worthwhile.
                //AlertArrival(this, astronaut);

                return true;
            }
            else {
                return false;
            }
        }

        public override bool Leave(AstroAI astronaut) {
            if (Assignees.Contains(astronaut)) {
                mySpriteRenderer.sprite = BridgeSprite;
                Assignees.Remove(astronaut);
                return true;
            }

            return false;
        }
    }
}