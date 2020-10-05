using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEditor.Compilation;

using UnityEngine;

public class BehaveStation : ActivityStation {

    // public float OffsetAngle = 12
    public enum BehaveStationStates {
        Claimed,
        Occupied,
        Abandoned
    }

    public BehaveStationStates currentState = BehaveStationStates.Abandoned;

    public void Abandon() {
        currentState = BehaveStationStates.Abandoned;
    }

    public void Arrived() {
        currentState = BehaveStationStates.Occupied;
    }
    
    protected override bool IsBehaviourStation() {
        return true;
    }

    void Awake() {
        AstroForeman.Single.Register(this);
    }
}


