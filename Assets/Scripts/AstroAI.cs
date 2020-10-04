using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

[RequireComponent(typeof(Creature))]
public class AstroAI : MonoBehaviour
{
    // Start is called before the first frame update
    public  AstroStats myStats;
    private Astronaut  myRotationData;
    public  bool       hasBeenChastised = false;
    public  bool       hasBeenKilled    = false;
    public  float      maxHitPoints     = 5f;
    public  float      currentHitPoints;
    
    void Start() {
        ConvertingToGood();
        myRotationData          = gameObject.GetComponent<Astronaut>();
    }

    // Update is called once per frame
    void Update()
    {
        //Transition
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //When close enough, start working
                if (IsAngularCloseEnough()) {
                    //Astronaut within range
                    myStats.myBehaveStation.currentState = BehaveStation.BehaveStationStates.Occupied;
                    myStats.myState                      = AstroStats.AIStates.Behaving;
                }
                //If too bored, start misbehaving
                MisbehaveCheck();
                break;
            case AstroStats.AIStates.Behaving:
                MisbehaveCheck();
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                //If you've been touched, go back to work
                if (hasBeenChastised) {
                    ConvertingToGood();
                }
                //When close enough, start being a mischievous little devil
                if (IsAngularCloseEnough()) {
                    myStats.myState = AstroStats.AIStates.Misbehaving;
                }
                break;
            case AstroStats.AIStates.Misbehaving:
                //If ya dead, ya dead
                if (hasBeenKilled) {
                    //Become unassigned from all stations
                    myStats.myState = AstroStats.AIStates.Dead;
                }
                break;
            case AstroStats.AIStates.Fixing:
                //Once the station is fixed, return to normal behavior
                if (myStats.myMisbehaveStation.currentState == MisbehaveStation.MisbehaveStationStates.Fixed) {
                    ConvertingToGood();
                }
                break;
            case AstroStats.AIStates.Dead:
                //Once you become dead, there's no escape. This transition is intentionally left blank
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Process
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //move towards station
                //Get more bored
                MisbehaveProgression();
                break;
            case AstroStats.AIStates.Behaving:
                //work it, gurl
                //Get more bored
                MisbehaveProgression();
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                //Move towards station
                break;
            case AstroStats.AIStates.Misbehaving:
                //Break the station a little
                //Good point of entry to add escalating difficulty
                //Logic is placeholder until balancing
                myStats.myMisbehaveStation.BreakUnit(Time.deltaTime);
                if (currentHitPoints <=0) {
                    hasBeenKilled = true;
                }
                break;
            case AstroStats.AIStates.Fixing:
                //Fix the machine a little
                myStats.myMisbehaveStation.RepairUnit(Time.deltaTime);
                break;
            case AstroStats.AIStates.Dead:
                //There is no afterlife to do tasks. This process is intentionally left blank
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Clean-up
        hasBeenChastised = false;
    }

    private void MisbehaveCheck() {
        if (myStats.timeUntilMisbehave <= 0f) {
            myStats.myMisbehaveStation = AstroForeman.Single.AssignMisbehavior(gameObject, myStats.myBehaveStation, myStats.myMisbehaveStation);
            myRotationData.SetTarget(myStats.myMisbehaveStation.transform.localPosition);
            myStats.myState   = AstroStats.AIStates.MoveToMisbehaving;
        }
    }

    private void MisbehaveProgression() {
        //Probably could use different numbers
        //This would also be where gradual difficulty increase could come into effect
        myStats.timeUntilMisbehave -= Time.deltaTime;
    }
    
    private bool IsAngularCloseEnough() {
        //return Math.Abs(myAngle-targetAngle) <= interactAngle
        return myRotationData.Distance2TargetAsAngle() <= myStats.interactAngle;
    }

    private float GetNewTargetAngle(ActivityStation newStation) {
        //Either the new Station has the angle baked in, or use a function to find the angle
        return 20f;
    }

    private void ConvertingToGood() {
        myStats.myBehaveStation = AstroForeman.Single.AssignBehavior(gameObject);
        myStats.myState         = AstroStats.AIStates.MoveToBehaving;
        currentHitPoints        = maxHitPoints;
    }
    private void OnMouseDown() {
        hasBeenChastised = true;
    }
}
