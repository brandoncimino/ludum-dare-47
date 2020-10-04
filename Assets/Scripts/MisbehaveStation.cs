using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

public class MisbehaveStation : ActivityStation
{
    public enum MisbehaveStationStates {
        Fixed,
        Broken
    }
    
    public BehaveStation behaveTwin;
    
    public override bool CanRegister() {
        return behaveTwin;
    }
    
    

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    void Awake() {
        remainingBreakTime = maxTimeToBreak;
    }
    
    
}
