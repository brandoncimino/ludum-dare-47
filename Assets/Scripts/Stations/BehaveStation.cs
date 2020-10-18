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
        else {
            throw new ArgumentException("Arrive: Astronaut arrived without claiming first!");
        }
    }

    public bool Claim(AstroAI astronaut = null) {
        if (currentState == BehaveStationStates.Abandoned) {
            currentState = BehaveStationStates.Claimed;
            OnTheirWay.Add(astronaut);
            return true;
        }
        else {
            return false;
        }
    }

    protected override bool IsBehaviourStation() {
        return true;
    }

    private void Start() {
        AstroForeman.Single.Register(this);
    }

    public override float DetermineConsequences(float timePassed) {
        if (currentState == BehaveStationStates.Occupied) {
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