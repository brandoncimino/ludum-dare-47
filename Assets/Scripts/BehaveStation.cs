using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using JetBrains.Annotations;

using UnityEditor.Compilation;

using UnityEngine;

public class BehaveStation : ActivityStation {

    public MisbehaveStation behaveTwin;
    public int           AssigneesMax = 1;
    // public float OffsetAngle = 12
    public enum BehaveStationStates {
        Claimed,
        Occupied,
        Abandoned
    }

    public BehaveStationStates currentState = BehaveStationStates.Abandoned;

    public override bool Leave(AstroAI astronaut) {
        
        if (Assignees.Contains(astronaut)) {
            Assignees.Remove(astronaut);
            
            if (Assignees.Count == 0) {
                currentState = BehaveStationStates.Abandoned;
            }

            return true;
        }

        return false;

    }

    public override bool Arrive(AstroAI astronaut) {
        if (Assignees.Count < AssigneesMax) {
            Assignees.Add(astronaut);
            currentState = BehaveStationStates.Occupied;
            return true;
        }
        else {
            return false;
        }
    }

    public bool Claim([CanBeNull] AstroAI astronaut = null) {
        if (currentState == BehaveStationStates.Abandoned) {
            currentState = BehaveStationStates.Claimed;
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
        if (behaveTwin.currentState == MisbehaveStation.MisbehaveStationStates.Broken) {
            // if the station next to you is broken, repair it first
            RepairTwin(timePassed);
            
            // while repairing, we cannot put active work into deceleration
            return 0;
        }

        // TODO: do "good" science
        
        // all other stations cause deceleration as positive work effect
        return -1;

    }
    
    private void RepairTwin(float deltaTime) {
        behaveTwin.Repair(deltaTime);
    }
}


