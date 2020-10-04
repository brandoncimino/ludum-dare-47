using System;
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
    
    //If this machine breaks, will it harm and eventually kill the astronaut?
    public bool isLethal = true;

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;
    
    //Time in seconds to fix
    public float maxTimeToFix = 5f;
    public float remainingFixTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    void Awake() {
        remainingBreakTime = maxTimeToBreak;
        remainingFixTime   = maxTimeToFix;
    }

    public void RepairUnit(float deltaTime) {
        remainingFixTime -= deltaTime;
        if (remainingFixTime <= 0) {
            remainingFixTime = maxTimeToFix;
            currentState     = MisbehaveStationStates.Fixed;
        }
    }

    
    public float BreakUnit(float deltaTime) {
        //This is where we would do unholy maths upon deltaTime to determine damages
        switch (currentState) {
            case MisbehaveStationStates.Fixed:
                remainingBreakTime -= deltaTime;
                if (remainingBreakTime <= 0) {
                    remainingBreakTime = maxTimeToBreak;
                    currentState       = MisbehaveStationStates.Broken;
                }
                return 0f;
                break;
            case MisbehaveStationStates.Broken:
                //Astronauts that are still at a location when it's broken slowly die
                return isLethal ? deltaTime : 0f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}
