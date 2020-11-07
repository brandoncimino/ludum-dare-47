using System;
using System.Collections.Generic;

using DefaultNamespace;
using DefaultNamespace.Text;

using UnityEngine;

using Random = System.Random;

public class MisbehaveStation : ActivityStation {
    // public float OffsetAngle = -12;

    protected Random Random = new Random();

    public BehaveStation behaveTwin;

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    #region information to do with warnings

    protected float WarnCounter      = 0;
    protected float MaxWaitTime      = 5f;
    protected bool  reportedWarning  = false;
    protected bool  canReportWarning = true;

    #endregion

    private void Start() {
        remainingBreakTime = maxTimeToBreak;
        AstroForeman.Single.Register(this);
    }

    private void Update() {
        // see if enough time has passed since last warning to regain warning ability
        if (!canReportWarning && !reportedWarning) {
            WarnCounter -= Time.deltaTime;
            if (WarnCounter <= 0) {
                // wait time has passed, can report again
                WarnCounter      = 0;
                canReportWarning = true;
            }
        }

        // check if it is worth reporting a warning and do so if needed
        if (canReportWarning && Assignees.Count > 0 && Reason2Warn()) {
            Scheduler.Single.ReportWarning(this);
            reportedWarning  = true;
            canReportWarning = false;
        }

        // TODO: change sprites to reflect state of the station
        // TODO: update health bars
    }

    /// <summary>
    /// repair this station. This increases the time it takes until it breaks and unleashes its "broken" behaviour (e.g. the monster).
    /// While a station is not fixed, any worker at the behave twin prioritizes repairs over deceleration and other positive consequences of their good behaviour.
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void Repair(float deltaTime) {
        remainingBreakTime += deltaTime;
        if (remainingBreakTime >= maxTimeToBreak) {
            remainingBreakTime = maxTimeToBreak;
            currentState       = MisbehaveStationStates.Fixed;
        }
    }

    protected override bool IsBehaveStation() {
        return false;
    }

    public override bool Arrive(AstroAI astronaut) {
        if (!Assignees.Contains(astronaut)) {
            Assignees.Add(astronaut);
            return Arrive_individual(astronaut);
        }

        return false;
    }

    protected virtual bool Arrive_individual(AstroAI astronaut) {
        return true;
    }

    public override bool Leave(AstroAI astronaut) {
        if (Assignees.Contains(astronaut)) {
            Assignees.Remove(astronaut);
            return Leave_individual(astronaut);
        }

        return false;
    }

    protected virtual bool Leave_individual(AstroAI astronaut) {
        return true;
    }


    /// <summary>
    /// This function tells us what happens from work at the station. It gets called from the space station with the time that has passed since the frame update. It return the acceleration (negative: deceleration) caused by the worker at this station. If the work has additional consequences than acceleration / deceleration, this is where they get called and come to life.
    /// </summary>
    /// <param name="timePassed">time passed since the last update</param>
    /// <returns>acceleration caused by this station (not weighted by the time)</returns>
    public override float DetermineConsequences(float timePassed) {
        // returns average acceleration over last time interval
        // this function gets overriden in the subclasses with specific negative consequences, but as a generic version for new misbehave stations in the future, Assignees.Count is a first good value.
        return Assignees.Count;
    }

    public override bool GiveWarning() {
        if (Reason2Warn()) {
            // reset warn counter
            reportedWarning = false;
            WarnCounter     = MaxWaitTime;

            // give warning
            StationLogger.Alert(
                new List<StationAlertType>() {StationAlertType.Console_Line_Station, StationInfo()},
                Alert.SeverityLevel.Warning,
                this
            );
            // TODO: create custom warning messages

            // warning has indeed happened
            return true;
        }

        // no reason for a warning was found, no warning has happened
        return false;
    }

    public override void GiveUpdate() {
        // actually, a misbehave station should never be called upon to give an info update
        behaveTwin.GiveUpdate();
    }

    public override Dictionary<string, string> GetAlertReplacements() {
        // TODO: include severity key
        var replacement = new Dictionary<string, string>() {
            {"ROOM", "UNKNOWN"},
            {"TIME", TimeStamp()}
        };

        // determine name of the room
        switch (DoorSign) {
            case ActivityRoom.Bridge:
                replacement["ROOM"] = "Bridge";
                break;
            case ActivityRoom.Rec:
                replacement["ROOM"] = "Gym";
                break;
            case ActivityRoom.Kitchen:
                replacement["ROOM"] = "Kitchen";
                break;
            case ActivityRoom.Lab:
                replacement["ROOM"] = "Lab";
                break;
            case ActivityRoom.Engine:
                replacement["ROOM"] = "Engine";
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    "DoorSign",
                    "Unknown room in GetAlertReplacements (Misbehave Station)"
                );
        }

        return replacement;
    }

    /// <summary>
    /// tells an astronaut's AI how to describe what they are doing based on the type of station they are at.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override StationAlertType AstronautInfo() {
        // see which room you are in and give the update alert accordingly
        switch (DoorSign) {
            case ActivityRoom.Bridge:
                return StationAlertType.Astronaut_Misbehave_Bridge;
            case ActivityRoom.Rec:
                return StationAlertType.Astronaut_Misbehave_Recreation;
            case ActivityRoom.Kitchen:
                return StationAlertType.Astronaut_Misbehave_Kitchen;
            case ActivityRoom.Lab:
                return StationAlertType.Astronaut_Misbehave_Lab;
            case ActivityRoom.Engine:
                return StationAlertType.Astronaut_Misbehave_Engine;
            default:
                throw new ArgumentOutOfRangeException("DoorSign", "unknown room detected for AstronautInfo");
        }
    }

    /// <summary>
    /// tells the station how to describe what they are doing (for an update) based on the type of station they are at.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override StationAlertType StationInfo() {
        // see which room you are in and give the update alert accordingly
        switch (DoorSign) {
            case ActivityRoom.Bridge:
                return StationAlertType.Room_Misbehave_Bridge;
            case ActivityRoom.Rec:
                return StationAlertType.Room_Misbehave_Recreation;
            case ActivityRoom.Kitchen:
                return StationAlertType.Room_Misbehave_Kitchen;
            case ActivityRoom.Lab:
                return StationAlertType.Room_Misbehave_Lab;
            case ActivityRoom.Engine:
                return StationAlertType.Room_Misbehave_Engine;
            default:
                throw new ArgumentOutOfRangeException("DoorSign", "unknown room detected for AstronautInfo");
        }
    }

    protected virtual bool Reason2Warn(float threshold = 0.7f) {
        // no reason to warn by default
        return false;
    }
}