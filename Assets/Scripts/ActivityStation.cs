using System;
using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace {
    public abstract class ActivityStation : MonoBehaviour {
        
        public List<AstroAI>  Assignees;
        public SpaceStation   home;
        public SpriteRenderer mySpriteRenderer;

        // positional and type information
        public float         PositionAngle;
        public int           PositionLayer;
        public ActivityRoom DoorSign;
        
        // the sprites for the different stations
        public Sprite BrideSprite;
        public Sprite KitchenSprite;
        public Sprite LabSprite;
        public Sprite RecSprite;
        private void Awake() {
            //Let foreman know you exist
            
        }

        
        public virtual bool CanRegister() {
            return true;
        }
        
        public void PlaceDown(float newAngle, int newLayer, SpaceStation newHome, ActivityRoom newDoorSign) {
            PositionAngle = newAngle;
            PositionLayer = newLayer;
            home          = newHome;
            DoorSign      = newDoorSign;

            PlaceSprite();

            var transform1 = transform;
            transform1.localScale *= transform1.parent.transform.localScale.x;
            
            transform.localPosition = new Vector3(
                (float) (home.radius * Math.Cos(PositionAngle)),
                -(home.depth / 2) + PositionLayer * home.depth / home.noAstronauts,
                (float) (home.radius * Math.Sin(PositionAngle))
            );
            
            transform.localEulerAngles = new Vector3(90, 0, PositionAngle+90);
            
            
        }

        public bool Occuppied => Assignees.Count != 0;
        
        /*public float GetNewTargetAngle() {
            //Either the new Station has the angle baked in, or use a function to find the angle
            return AstronautVisuals.SetTarget(gameObject.transform.position);
        }
        */

        protected void PlaceSprite() {
        
            switch (DoorSign) {
                case ActivityRoom.Bridge:
                    mySpriteRenderer.sprite = BrideSprite;
                    break;
                case ActivityRoom.Kitchen:
                    mySpriteRenderer.sprite = KitchenSprite;
                    break;
                case ActivityRoom.Lab:
                    mySpriteRenderer.sprite = LabSprite;
                    break;
                case ActivityRoom.Rec:
                    mySpriteRenderer.sprite = RecSprite;
                    break;
                default:
                    mySpriteRenderer.sprite = BrideSprite;
                    break;
            }
        }
    }
}