using System;
using System.Collections.Generic;

using DefaultNamespace;
using DefaultNamespace.Text;

public class BehaveStation : ActivityStation {
    public MisbehaveStation behaveTwin;
    public List<AstroAI>    OnTheirWay;

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

            //alert that behaving has started
            //AlertArrival(this, astronaut);

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

    protected override bool IsBehaviourStation() {
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

    public override void GiveWarning() {
        // TODO: implement how warnings work
        GiveUpdate();
    }

    public override void GiveUpdate() {
        // TODO: put in the replacements
        StationLogger.Alert(GetAlert(DoorSign, true), Alert.SeverityLevel.Info);
    }
}