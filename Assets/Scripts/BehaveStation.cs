using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using JetBrains.Annotations;

using UnityEditor.Compilation;

using UnityEngine;

public class BehaveStation : ActivityStation {

    public MisbehaveStation behaveTwin;
    public List<AstroAI>    OnTheirWay;
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

            // TODO: do "good" science
        
            // all other stations cause deceleration as positive work effect
            return -1;
        }

        return 0;
    }
    
    private void RepairTwin(float deltaTime) {
        behaveTwin.Repair(deltaTime);
    }
}


