using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using JetBrains.Annotations;

using UnityEditor.Compilation;

using UnityEngine;

public class BehaveStation : ActivityStation {

    public int AssigneesMax = 1;
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
            AssigneesCount--;
            
            if (AssigneesCount == 0) {
                currentState = BehaveStationStates.Abandoned;
            }

            return true;
        }

        return false;

    }

    public override bool Arrive(AstroAI astronaut) {
        if (AssigneesCount < AssigneesMax) {
            Assignees.Add(astronaut);
            currentState = BehaveStationStates.Occupied;
            AssigneesCount++;
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
}


