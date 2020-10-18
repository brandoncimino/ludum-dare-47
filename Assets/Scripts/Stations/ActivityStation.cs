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
        public    float        PositionAngle;
        protected float        OffsetDistance;
        public    float        OffsetAngle;
        protected int          PositionLayer;
        public    ActivityRoom DoorSign;

        /// <summary>
        /// This function positions the activity station on the circular space station, and rotates it accordingly. For a position, it needs the angle of the room's center and the layer (depth) at which to place the station. The station sprite will be offset from the center of the room by the variable OffsetAngle, which could be specified in each subclass individually. The call of PlaceDownCorrections admits individual corrections based on the activity station subclass, such as different scaling of the sprites. The rotation toward the center of the station is based on the center of the room (replacing the circular station with a hexagon wouldn't change anything about the placements here.)
        /// </summary>
        /// <param name="newAngle">center or the room</param>
        /// <param name="newLayer">depth layer into the station</param>
        public void PlaceDown(float newAngle, int newLayer) {
            // get the base positional information
            PositionAngle = (newAngle + OffsetAngle) % 360;
            PositionLayer = newLayer;
            home          = SpaceStation.Single;

            // how far does the activity station be moved from the center of the room
            OffsetDistance = home.radius * ((float) Math.PI) * OffsetAngle / 180f;
            PlaceDownCorrection(); // also corrects offsetDistance for some room types

            Transform transform1 = transform;
            var       angle      = Math.PI * (newAngle) / 180;

            // if the transform was changed after the class instances came to life, the sprite scale needs to be changed like this:
            //(transform1 = transform).localScale *= transform1.parent.transform.localScale.x;

            // position at the center of the room
            transform1.localPosition = new Vector3(
                (float) (home.radius * Math.Cos(angle)),
                -(home.depth / 2) + PositionLayer * home.depth / home.noLayers,
                (float) (home.radius * Math.Sin(angle))
            );

            // rotate so that it sits on the floor at the center of the room
            transform1.localEulerAngles = new Vector3(90, 0, newAngle + 90);

            // move sideways, keeping the slope of the floor from before
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
        protected abstract bool IsBehaveStation();
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

        public Dictionary<string, string> GetAlertReplacements() {
            return StationLogger.SoupOfTheDay.GetAlertReplacements();
        }

        public abstract StationAlertType AstronautInfo();
    }
}