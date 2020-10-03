using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

public class BehaveStationStats : ActivityStation
{
    public enum BehaveStationStates {
        Occupied,
        Abandoned
    }

    public BehaveStationStates currentState = BehaveStationStates.Abandoned;

    

    public void Abandon() {
        currentState = BehaveStationStates.Abandoned;
    }
}
