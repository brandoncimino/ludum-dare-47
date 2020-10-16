using System;
using System.Collections.Generic;

using DefaultNamespace.Text;

using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

namespace DefaultNamespace {
    public abstract class ActivityStation : MonoBehaviour, IAlertReplacements {
        public List<AstroAI>  Assignees;
        public SpaceStation   home = SpaceStation.Single;
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

        public void PlaceDown(float newAngle, int newLayer, ActivityRoom newDoorSign) {
            DoorSign      = newDoorSign;
            PositionAngle = (newAngle + OffsetAngle) % 360;
            PositionLayer = newLayer;
            home          = SpaceStation.Single;

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
                (float) (-OffsetDistance * Math.Sin(angle)),
                0,
                (float) (+OffsetDistance * Math.Cos(angle))
            );
        }

        public bool Occuppied => Assignees.Count != 0;

        protected void NameOnDoor() {
            switch (DoorSign) {
                case ActivityRoom.Bridge:
                    mySpriteRenderer.sprite = BridgeSprite;
                    if (!IsBehaviourStation()) {
                        PositionLayer -= 1;
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
                        transform.localScale *= 1.5f;
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

        public abstract float DetermineConsequences(float timePassed);

        public static StationAlertType GetAlert(ActivityRoom room, bool isBehaveStation) {
            switch (room) {
                case ActivityRoom.Bridge:
                    return isBehaveStation ? StationAlertType.Behave_Bridge : StationAlertType.Misbehave_Bridge;
                case ActivityRoom.Rec:
                    return isBehaveStation ? StationAlertType.Behave_Recreation : StationAlertType.Misbehave_Recreation;
                case ActivityRoom.Kitchen:
                    return isBehaveStation ? StationAlertType.Behave_Kitchen : StationAlertType.Misbehave_Kitchen;
                case ActivityRoom.Lab:
                    return isBehaveStation ? StationAlertType.Behave_Lab : StationAlertType.Misbehave_Lab;
                case ActivityRoom.Engine:
                    return isBehaveStation ? StationAlertType.Behave_Engine : StationAlertType.Misbehave_Engine;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(room), room);
            }
        }

        protected void AlertArrival(params IAlertReplacements[] replacementSources) {
            StationLogger.Alert(GetAlert(DoorSign, IsBehaviourStation()), replacementSources);
        }

        public abstract bool Arrive(AstroAI astronaut);

        public abstract bool Leave(AstroAI astronaut);

        public Dictionary<string, string> GetAlertReplacements() {
            return StationLogger.SoupOfTheDay.GetAlertReplacements();
        }
    }
}