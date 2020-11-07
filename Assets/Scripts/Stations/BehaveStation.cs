using System;
using System.Collections.Generic;

using DefaultNamespace;
using DefaultNamespace.Text;

public class BehaveStation : ActivityStation {
    public MisbehaveStation behaveTwin;
    public List<AstroAI>    OnTheirWay;
    //protected int              UpdateDataCount = 0;

    public BehaveStationStates currentState = BehaveStationStates.Abandoned;

    public override bool Leave(AstroAI astronaut) {
        if (Assignees.Contains(astronaut) || OnTheirWay.Contains(astronaut)) {
            Assignees.Remove(astronaut);
            OnTheirWay.Remove(astronaut);

            if (Assignees.Count + OnTheirWay.Count == 0) {
                currentState = BehaveStationStates.Abandoned;
            }

            return true;
        }

        return false;
    }

    public override bool Arrive(AstroAI astronaut) {
        if (OnTheirWay.Contains(astronaut)) {
            OnTheirWay.Remove(astronaut);
            Assignees.Add(astronaut);
            currentState = BehaveStationStates.Occupied;

            return true;
        }

        throw new ArgumentException("Arrive: Astronaut arrived without claiming first!");
    }

    public bool Claim(AstroAI astronaut = null) {
        if (currentState == BehaveStationStates.Abandoned) {
            currentState = BehaveStationStates.Claimed;
            OnTheirWay.Add(astronaut);
            return true;
        }

        return false;
    }

    protected override bool IsBehaveStation() {
        return true;
    }

    private void Start() {
        // informs the foreman about this object
        AstroForeman.Single.Register(this);
    }

    /// <summary>
    /// This function tells us what happens from work at the station. It gets called from the space station with the time that has passed since the frame update. It return the acceleration (negative: deceleration) caused by the worker at this station. If the work has additional consequences than acceleration / deceleration, this is where they get called and come to life.
    /// </summary>
    /// <param name="timePassed">time passed since the last update</param>
    /// <returns>acceleration caused by this station (not weighted by the time)</returns>
    public override float DetermineConsequences(float timePassed) {
        if (currentState == BehaveStationStates.Occupied) {
            // there are only consequences if someone is here
            if (behaveTwin.currentState != MisbehaveStationStates.Fixed) {
                // if the station next to you is broken, repair it first
                RepairTwin(timePassed);

                // while repairing, we cannot put active work into deceleration
                return 0;
            }

            return -1f;
        }

        return 0;
    }

    protected void RepairTwin(float deltaTime) {
        // the misbehave station is the one that can break or be repaired
        behaveTwin.Repair(deltaTime);
    }

    public override bool GiveWarning() {
        // TODO: implement how warnings work
        GiveUpdate();
        return true;
    }

    public override void GiveUpdate() {
        // collect the different parts of the update in a list
        var alerts = new List<StationAlertType>() {StationAlertType.Console_Line_Station};
        alerts.AddRange(GiveUpdate_DataLines());
        alerts.AddRange(GiveUpdate_Usage());
        alerts.AddRange(behaveTwin.GiveUpdate_Usage());

        // collect every instance that might know how a placeholder in the alert shall be replaced
        var replacements = new List<IAlertReplacements>() {
            this
        };
        if (Assignees.Count > 0) {
            replacements.Add(Assignees[0]);
        }

        StationLogger.Alert(alerts, Alert.SeverityLevel.Info, replacements.ToArray());
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
                return StationAlertType.Astronaut_Behave_Bridge;
            case ActivityRoom.Rec:
                return StationAlertType.Astronaut_Behave_Recreation;
            case ActivityRoom.Kitchen:
                return StationAlertType.Astronaut_Behave_Kitchen;
            case ActivityRoom.Lab:
                return StationAlertType.Astronaut_Behave_Lab;
            case ActivityRoom.Engine:
                return StationAlertType.Astronaut_Behave_Engine;
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
                return StationAlertType.Room_Behave_Bridge;
            case ActivityRoom.Rec:
                return StationAlertType.Room_Behave_Recreation;
            case ActivityRoom.Kitchen:
                return StationAlertType.Room_Behave_Kitchen;
            case ActivityRoom.Lab:
                return StationAlertType.Room_Behave_Lab;
            case ActivityRoom.Engine:
                return StationAlertType.Room_Behave_Engine;
            default:
                throw new ArgumentOutOfRangeException("DoorSign", "unknown room detected for AstronautInfo");
        }
    }

    protected virtual IEnumerable<StationAlertType> GiveUpdate_DataLines() {
        var dataLines = new List<StationAlertType>();
        switch (UpdateDataCount) {
            case 0:
                break;
            case 1:
                dataLines.Add(StationAlertType.Data_1);
                break;
            case 2:
                dataLines.Add(StationAlertType.Data_2);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    "UpdateDataCount",
                    "did not account for this much data to be shown in the scheduled update."
                );
        }

        return dataLines;
    }

    protected virtual int UpdateDataCount => 0;
}