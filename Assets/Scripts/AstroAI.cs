using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroAI : MonoBehaviour
{
    // Start is called before the first frame update
    public AstroStats   myStats;
    public AstroForeman Foreman;
    
    void Start() {
        myStats.myBehaveStation = Foreman.AssignBehavior(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Transition
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //When close enough, start working
                if (Vector3.Distance(transform.localPosition,myStats.targetLocation) < myStats.interactDistance) {
                    //Astronaut within range
                    myStats.myState = AstroStats.AIStates.Behaving;
                }
                //If too bored, start misbehaving
                if (myStats.timeUntilMisbehave <= 0f) {
                    myStats.myMisbehaveStation = Foreman.AssignMisbehavior(gameObject, myStats.myBehaveStation, myStats.myMisbehaveStation);
                    myStats.myState   = AstroStats.AIStates.MoveToMisbehaving;
                }
                break;
            case AstroStats.AIStates.Behaving:
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                break;
            case AstroStats.AIStates.Misbehaving:
                break;
            case AstroStats.AIStates.Fixing:
                break;
            case AstroStats.AIStates.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Process
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //move towards station
                break;
            case AstroStats.AIStates.Behaving:
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                break;
            case AstroStats.AIStates.Misbehaving:
                break;
            case AstroStats.AIStates.Fixing:
                break;
            case AstroStats.AIStates.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
