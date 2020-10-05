using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

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
    public float timeUntilMisbehave;
    //Time until Astronaut dies due to misbehaving
    public float timeUntilDeath = 5f;
    //Location that I want to move towards
    public float targetAngle;
    
    //Distance until I can interact with something
    public float interactAngle = .05f;
    //Station that I am navigating towards, behaving at, or misbehaving at
    public  BehaveStation    myBehaveStation;
    [CanBeNull] public MisbehaveStation myMisbehaveStation = null;

    //Unused. Will be used to randomly generate my space suit's color. Poorly executed, probably.
    //public Color myColor = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
    void Awake() {
        timeUntilMisbehave = Random.Range(20.0f, 45.0f);
    }
}
