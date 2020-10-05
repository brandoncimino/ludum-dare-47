using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(Astronaut))]
public class AstroAI : MonoBehaviour
{
    // Start is called before the first frame update
    public  AstroStats myStats;
    public  Astronaut  myRotationData;
    public  bool       hasBeenChastised   = false;
    private float      remainingFleeTime  = 0;
    private float      maxFleeTime        = 5.0f;
    private float      timeBeforeDelete = 5f;

    void Start() {
        myStats        = gameObject.GetComponent<AstroStats>();
        myRotationData = gameObject.GetComponent<Astronaut>();
        ConvertingToGood();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myRotationData.alive) {
            //If ya dead, ya dead
            myStats.myState = AstroStats.AIStates.Dead;
            
            //Become unassigned from all stations
            myStats.myBehaveStation.Leave(this);
            myStats.myMisbehaveStation.Leave(this);
        }
        
        //Transition
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //When close enough, start working
                if (IsAngularCloseEnough()) {
                    //Astronaut within range
                    myRotationData.ChangeThought(Thought.Boredom);
                    myStats.myBehaveStation.Arrive(this);
                    myStats.myState = AstroStats.AIStates.Behaving;
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
                    myStats.myMisbehaveStation.Arrive(this);
                    myStats.myState = AstroStats.AIStates.Misbehaving;
                }
                break;
            case AstroStats.AIStates.Misbehaving:
                
                if (hasBeenChastised) {
                    ConvertingToGood();
                }
                
                break;
            case AstroStats.AIStates.Fixing:
                //Once the station is fixed, return to normal behavior
                if (myStats.myMisbehaveStation.currentState == MisbehaveStation.MisbehaveStationStates.Fixed) {
                    ConvertingToGood();
                }
                break;
            case AstroStats.AIStates.Fleeing:
                remainingFleeTime = Math.Max(remainingFleeTime - Time.deltaTime, 0);
                if (remainingFleeTime == 0) {
                    StopFleeing();
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
                //move towards station (executed by Astronaut:Creature's Update(); )
                //Get more bored
                MisbehaveProgression();
                break;
            case AstroStats.AIStates.Behaving:
                //work it, gurl
                //Get more bored
                MisbehaveProgression();
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                //Move towards station (executed by Astronaut:Creature's Update(); )
                break;
            case AstroStats.AIStates.Misbehaving:
                // conseqences of misbehaving are determined in the station, nothing active to do here
                break;
            case AstroStats.AIStates.Fixing:
                //Fix the machine a little
                // Nicole: changed so that repairing is part of behaving - no separate state needed
                // myStats.myMisbehaveStation.RepairUnit(Time.deltaTime);
                break;
            case AstroStats.AIStates.Fleeing:
                break;
            case AstroStats.AIStates.Dead:
                //There is no afterlife to do tasks. This process is intentionally left blank
                timeBeforeDelete -= Time.deltaTime;
                if (timeBeforeDelete < 0) {
                    myRotationData.home.Astronauts.Remove(myRotationData);
                    Destroy(this.gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Clean-up
        hasBeenChastised = false;
    }

    private void MisbehaveCheck() {
        if (myStats.timeUntilMisbehave <= 0f) {
            myRotationData.ChangeThought(Thought.Mischief);
            myStats.myMisbehaveStation = AstroForeman.Single.AssignMisbehavior(this, myStats.myBehaveStation, myStats.myMisbehaveStation);
            GetNewTargetAngle(myStats.myMisbehaveStation);
            myRotationData.SetTarget(myStats.myMisbehaveStation.PositionAngle);
            myStats.myState   = AstroStats.AIStates.MoveToMisbehaving;
            myStats.myBehaveStation.Leave(this);
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

    private void GetNewTargetAngle(ActivityStation newStation) {
        //Either the new Station has the angle baked in, or use a function to find the angle
        myRotationData.SetTarget(newStation.PositionAngle);
    }

    private void ConvertingToGood() {
        //Debug.Log(AstroForeman.Single);
        myRotationData.ChangeThought(Thought.Memes);

        // leave the misbehave station if there
        myStats.myMisbehaveStation?.Leave(this);
        
        // find new good behaviour
        myStats.myBehaveStation = AstroForeman.Single.AssignBehavior(this);
        GetNewTargetAngle(myStats.myBehaveStation);
        myStats.myState            = AstroStats.AIStates.MoveToBehaving;
        myRotationData.Heal();
        myStats.timeUntilMisbehave = Random.Range(15f, 23f);
    }
    private void OnMouseDown() {
        hasBeenChastised = true;
    }

    public void StartFleeing(bool monsterMovingRight) {
        myStats.myState             =  AstroStats.AIStates.Fleeing;
        myRotationData.FleeingRight =  monsterMovingRight;
        remainingFleeTime           =  maxFleeTime;
        myRotationData.ChangeThought(Thought.Alarm);
        
        //Become unassigned from all stations
        myStats.myBehaveStation.Leave(this);
        myStats.myMisbehaveStation?.Leave(this);
    }

    private void StopFleeing() {
        ConvertingToGood();
    }

}
