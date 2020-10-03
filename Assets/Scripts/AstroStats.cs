using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AstroStats : MonoBehaviour
{
    public enum AIStates
    {
        //Doing Nothing
        Idle,
        //Have a Task
        Tasked,
        //Alive and outside the Station
        Ejecting,
        //Dead and inside the Station
        DeadInside,
        //Dead and outside the Station
        DeadOutside
    }
    //Start Idle
    public AIStates myState = AIStates.Idle;
    //Time until Idle fidgeting
    public float maxIdleTime = Random.Range(4.0f, 6.6f);
    //Time elapsed until Idle fidgeting
    public float currentIdleWait = 0f;
    //Location that I want to move towards
    public Vector3 targetLocation;
    
    //Distance until I can interact with something
    public static float interactDistance = .05f;
    //Am I inside the Station? Used to transition from DeadInside to DeadOutside
    public bool isInside = true;
    
    //Unused. Will be used to randomly generate my space suit's color. Poorly executed, probably.
    public Color myColor = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
}
