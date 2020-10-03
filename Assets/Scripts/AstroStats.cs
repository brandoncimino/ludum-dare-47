using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AstroStats : MonoBehaviour
{
    public enum AIStates
    {
        MoveToBehaving,
        Behaving,
        MoveToMisbehaving,
        Misbehaving,
        Fixing,
        Dead
    }
    //Start Idle
    public AIStates myState = AIStates.MoveToBehaving;
    //Time until Astronaut misbehaves
    public float timeUntilMisbehave = Random.Range(20.0f, 45.0f);
    //Location that I want to move towards
    public Vector3 targetLocation;
    
    //Distance until I can interact with something
    public float interactDistance = .05f;
    //Station that I am navigating towards, behaving at, or misbehaving at
    public BehaveStationStats    myBehaveStation;
    public MisbehaveStationStats myMisbehaveStation;

    //Unused. Will be used to randomly generate my space suit's color. Poorly executed, probably.
    public Color myColor = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
}
