using System;
using System.Collections.Generic;

using UnityEngine;

namespace DefaultNamespace {
    public abstract class ActivityStation : MonoBehaviour {
        
        public List<AstroAI>  Assignees;
        public SpaceStation   home;
        public SpriteRenderer mySpriteRenderer;

        // positional and type information
        public float        PositionAngle;
        public float        OffsetDistance;
        public float        OffsetAngle;
        public int          PositionLayer;
        public ActivityRoom DoorSign;
        
        // the sprites for the different stations
        public Sprite BridgeSprite;
        public Sprite KitchenSprite;
        public Sprite LabSprite;
        public Sprite RecSprite;
        public Sprite EngineSprite;
        private void Awake() {
            //Let foreman know you exist
            
        }

        
        public virtual bool CanRegister() {
            return true;
        }
        
        public void PlaceDown(float newAngle, int newLayer, SpaceStation newHome, ActivityRoom newDoorSign) {
            
            DoorSign       = newDoorSign;
            PositionAngle  = (newAngle + OffsetAngle) % 360;
            PositionLayer  = newLayer;
            home           = newHome;
            
            OffsetDistance = home.radius * ((float) Math.PI) * OffsetAngle / 180f;
            NameOnDoor(); // also corrects offsetDistance for some room types
            // OffsetDistance = 0.2f;
            
            Transform transform1;
            var       angle = Math.PI * (newAngle) / 180;
            
            (transform1 = transform).localScale *= transform1.parent.transform.localScale.x;
            
            transform1.localPosition = new Vector3(
                (float) (home.radius * Math.Cos(angle)),
                -(home.depth / 2) + PositionLayer * home.depth / home.noLayers,
                (float) (home.radius * Math.Sin(angle))
            );

            transform1.localEulerAngles = new Vector3(90, 0, newAngle + 90);

            transform1.localPosition += new Vector3(
                (float) (- OffsetDistance * Math.Sin(angle)),
                0,
                (float) (+ OffsetDistance * Math.Cos(angle))
            );
            
        }

        public bool Occuppied => Assignees.Count != 0;
        
        /*public float GetNewTargetAngle() {
            //Either the new Station has the angle baked in, or use a function to find the angle
            return AstronautVisuals.SetTarget(gameObject.transform.position);
        }
        */

        protected void NameOnDoor() {
        
            switch (DoorSign) {
                case ActivityRoom.Bridge:
                    mySpriteRenderer.sprite = BridgeSprite;
                    if (!IsBehaviourStation()) {
                        OffsetDistance       =  0;
                        PositionLayer        -= 1;
                    }
                    break;
                case ActivityRoom.Kitchen:
                    mySpriteRenderer.sprite = KitchenSprite;
                    break;
                case ActivityRoom.Lab:
                    mySpriteRenderer.sprite = LabSprite;
                    break;
                case ActivityRoom.Rec:
                    mySpriteRenderer.sprite = RecSprite;
                    if (!IsBehaviourStation()) {
                        OffsetDistance       =  0;
                        transform.localScale *= 2;
                        PositionLayer        -= 1;
                    }
                    else {
                        transform.localScale *= 1.2f;
                    }
                    break;
                case ActivityRoom.Engine:
                    mySpriteRenderer.sprite = EngineSprite;
                    break;
                default:
                    mySpriteRenderer.sprite = BridgeSprite;
                    OffsetAngle             = 0;
                    break;
            }
        }

        protected virtual bool IsBehaviourStation() {
            throw new NotImplementedException("needs to be implemented in subclass");
        }

        public virtual float DetermineConsequences(float timePassed) {
            // returns average acceleration over last time interval
            // here, accelation is modelled as piecewise constant - hence timePassed cancels out
            return Assignees.Count * ( IsBehaviourStation() ? -1.0f : +1.0f);
        }

        public virtual bool Arrive(AstroAI astronaut) {
            if (!Assignees.Contains(astronaut)) {
                Assignees.Add(astronaut);
                return true;
            }
            else {
                return false;
            }
        }
        
        public virtual bool Leave(AstroAI astronaut) {
        
            if (Assignees.Contains(astronaut)) {
                Assignees.Remove(astronaut);
                return true;
            }

            return false;

        }
        
    }
}