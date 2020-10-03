using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroAI : MonoBehaviour
{
    // Start is called before the first frame update
    public AstroStats myStats;
    
    void Start()
    {
        //Start where you want to be
        myStats.targetLocation = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        switch (myStats.myState)
        {
            case AstroStats.AIStates.Idle:
                if (Vector3.Distance(transform.position,myStats.targetLocation)<AstroStats.interactDistance)
                {
                    myStats.currentIdleWait += Time.deltaTime;
                    if (myStats.currentIdleWait >= myStats.maxIdleTime)
                    {
                        //TODO: Randomly generate new location into "targetLocation"
                        myStats.currentIdleWait = 0f;
                    }
                }
                else
                {
                    //TODO: Move along ground towards "targetLocation"
                }
                break;
            case AstroStats.AIStates.Tasked:
                //TODO: Approach, interact, and complete task.
                //TODO: After approach, if task is already being worked on, quit
                break;
            case AstroStats.AIStates.Ejecting:
                //Thrash uncontrollably, but Death is approaching
                //TODO: If enough time has passed, transition to DeadOutside
                break;
            case AstroStats.AIStates.DeadInside:
                if (!myStats.isInside)
                {
                    myStats.myState = AstroStats.AIStates.DeadOutside;
                }
                //Lie uncomfortably still
                break;
            case AstroStats.AIStates.DeadOutside:
                //Careen gently away from space station
                //TODO: Obtain direction away from center of station. Float that way
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void DyingInside()
    {
        if (myStats.myState!=AstroStats.AIStates.DeadInside || myStats.myState!=AstroStats.AIStates.DeadOutside)
        {
            //Oops, I died on the ship
            myStats.myState = AstroStats.AIStates.DeadInside;
            //TODO: Send Foreman resignation letter. effective immediately
            //TODO: Send Foreman new Task: Stuff my corpse in airlock
        }
    }
    
    //TODO: May need PinkSlip function to be fired (Tasked >> Idle)
}
