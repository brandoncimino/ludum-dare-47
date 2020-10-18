using System;
using System.Collections.Generic;

using DefaultNamespace.Text;

using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

namespace DefaultNamespace {
    public abstract class ActivityStation : MonoBehaviour, IAlertReplacements, IMessengers {
        public List<AstroAI>  Assignees;
        public SpaceStation   home = SpaceStation.Single;
        public SpriteRenderer mySpriteRenderer;
        public StationLogger  StationLogger = StationLogger.Single;

        // positional and type information
        public float        PositionAngle;
        public float        OffsetDistance;
        public float        OffsetAngle;
        public int          PositionLayer;
        public ActivityRoom DoorSign;

        public void PlaceDown(float newAngle, int newLayer) {
            PositionAngle = (newAngle + OffsetAngle) % 360;
            PositionLayer = newLayer;
            home          = SpaceStation.Single;

            OffsetDistance = home.radius * ((float) Math.PI) * OffsetAngle / 180f;
            PlaceDownCorrection(); // also corrects offsetDistance for some room types

            Transform transform1 = transform;
            var       angle      = Math.PI * (newAngle) / 180;

            //(transform1 = transform).localScale *= transform1.parent.transform.localScale.x;

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

        protected virtual void PlaceDownCorrection() {
            // option for subclasses to change the the offset distance and the layer individually
        }

        public abstract bool Arrive(AstroAI astronaut);

        public abstract bool Leave(AstroAI astronaut);

        public abstract void GiveWarning();
        public abstract void GiveUpdate();
        protected abstract bool IsBehaviourStation();
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

        public Dictionary<string, string> GetAlertReplacements() {
            return StationLogger.SoupOfTheDay.GetAlertReplacements();
        }
    }
}