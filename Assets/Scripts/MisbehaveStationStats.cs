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
    
    public static GameObject         BehaveTwin;
    private       BehaveStationStats BehaveTwinStats = BehaveTwin.GetComponent<BehaveStationStats>();

    public BehaveStationStats GetTwinStats() {
        return BehaveTwinStats;
    }
    
    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;
}
