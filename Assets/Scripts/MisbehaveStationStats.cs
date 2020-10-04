using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

public class MisbehaveStationStats : ActivityStation
{
    public enum MisbehaveStationStates {
        Fixed,
        Broken
    }
    
    public BehaveStationStats behaveTwin;
    
    public override bool CanRegister() {
        return behaveTwin;
    }
    
    

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;
}
