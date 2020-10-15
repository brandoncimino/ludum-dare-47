using System;
using System.Collections.Generic;

using DefaultNamespace;

public class BehaveStation : ActivityStation {
    public  MisbehaveStation behaveTwin;
    public  List<AstroAI>    OnTheirWay;
    private float            IncarnationProcess = 0;
    private float            IncarnationTime    = 30f;
    public  bool             MonsterInStorage   = true;

    // public float OffsetAngle = 12
    public enum BehaveStationStates {
        Claimed,
        Occupied,
        Abandoned
    }

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
            AlertArrival(this, astronaut);

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

    void Awake() {
        AstroForeman.Single.Register(this);
    }

    public override float DetermineConsequences(float timePassed) {
        if (currentState == BehaveStationStates.Occupied) {
            if (behaveTwin.currentState != MisbehaveStation.MisbehaveStationStates.Fixed) {
                // if the station next to you is broken, repair it first
                RepairTwin(timePassed);

                // while repairing, we cannot put active work into deceleration
                return 0;
            }

            switch (DoorSign) {
                case ActivityRoom.Bridge:
                    return -1.5f;
                case ActivityRoom.Rec:
                    return -1f;
                case ActivityRoom.Kitchen:
                    return -1f;
                case ActivityRoom.Lab:
                    // do "good" science
                    return ScienceUp(timePassed);
                case ActivityRoom.Engine:
                    return -2f;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return 0;
    }

    private void RepairTwin(float deltaTime) {
        behaveTwin.Repair(deltaTime);
    }

    private float ScienceUp(float timePassed) {
        IncarnationProcess += timePassed;

        if (IncarnationProcess >= IncarnationTime) {
            home.SpawnAstronaut(PositionAngle);
            IncarnationProcess = 0;
            MonsterInStorage   = false;
        }

        return 0;
    }
}