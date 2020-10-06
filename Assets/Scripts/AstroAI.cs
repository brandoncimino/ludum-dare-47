using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DefaultNamespace;

using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(Astronaut))]
public class AstroAI : CreatureAI
{
    // Start is called before the first frame update
    public  AstroStats myStats;
    public  Astronaut  myBody;
    private float      remainingFleeTime  = 0;
    private float      maxFleeTime        = 5.0f;
    private float      timeBeforeDelete = 5f;

    void Start() {
        myStats        = gameObject.GetComponent<AstroStats>();
        myBody = gameObject.GetComponent<Astronaut>();
        ConvertingToGood();
    }

    // Update is called once per frame
    private void Update()
    {
        
        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                //Get more bored
                MisbehaveProgression();
                //If too bored, start misbehaving
                MisbehaveCheck();
                break;
            case AstroStats.AIStates.Behaving:
                //work it, gurl
                //Get more bored
                MisbehaveProgression();
                MisbehaveCheck();
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                break;
            case AstroStats.AIStates.Misbehaving:
                break;
            case AstroStats.AIStates.Fixing:
                break;
            case AstroStats.AIStates.Fleeing:
                remainingFleeTime = Math.Max(remainingFleeTime - Time.deltaTime, 0);
                if (remainingFleeTime == 0) {
                    StopFleeing();
                }
                break;
            case AstroStats.AIStates.Dead:
                //There is no afterlife to do tasks.
                timeBeforeDelete -= Time.deltaTime;
                if (timeBeforeDelete < 0) {
                    myBody.home.Astronauts.Remove(myBody);
                    Destroy(this.gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void MisbehaveCheck() {
        if (myStats.timeUntilMisbehave <= 0f) {
            myBody.ChangeThought(Thought.Mischief);
            myStats.myMisbehaveStation = AstroForeman.Single.AssignMisbehavior(this, myStats.myBehaveStation, myStats.myMisbehaveStation);
            GetNewTargetAngle(myStats.myMisbehaveStation);
            myBody.SetTarget(myStats.myMisbehaveStation.PositionAngle);
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
        return myBody.Distance2TargetAsAngle() <= myStats.interactAngle;
    }

    private void GetNewTargetAngle(ActivityStation newStation) {
        //Either the new Station has the angle baked in, or use a function to find the angle
        myBody.SetTarget(newStation.PositionAngle);
    }

    private void ConvertingToGood() {
        //Debug.Log(AstroForeman.Single);
        myBody.ChangeThought(Thought.Memes);

        // leave the misbehave station if there
        myStats.myMisbehaveStation?.Leave(this);
        
        // find new good behaviour
        myStats.myBehaveStation = AstroForeman.Single.AssignBehavior(this);
        GetNewTargetAngle(myStats.myBehaveStation);
        myStats.myState            = AstroStats.AIStates.MoveToBehaving;
        myBody.Heal();
        myStats.timeUntilMisbehave = Random.Range(15f, 23f);
    }
    private void OnMouseDown() {
        if (myStats.myState == AstroStats.AIStates.Misbehaving ||
            myStats.myState == AstroStats.AIStates.MoveToMisbehaving) {
            ConvertingToGood();
        }
    }

    public override void StartFleeing(bool monsterMovingRight = true) {
        myStats.myState             =  AstroStats.AIStates.Fleeing;
        myBody.FleeingRight =  monsterMovingRight;
        remainingFleeTime           =  maxFleeTime;
        myBody.ChangeThought(Thought.Alarm);
        myBody.fleeing = true;
        
        //Become unassigned from all stations
        myStats.myBehaveStation.Leave(this);
        myStats.myMisbehaveStation?.Leave(this);
    }

    private void StopFleeing() {
        myBody.fleeing = false;
        ConvertingToGood();
    }

    public override void AnnounceArrival() {
        if (myStats.myState == AstroStats.AIStates.MoveToBehaving) {
            myStats.myState = AstroStats.AIStates.Behaving;
            myBody.ChangeThought(Thought.Boredom);
            myStats.myBehaveStation.Arrive(this);
            return;
        }
        if (myStats.myState == AstroStats.AIStates.MoveToMisbehaving) {
            myStats.myState = AstroStats.AIStates.Misbehaving;
            myStats.myMisbehaveStation.Arrive(this);
            return;
        }
        throw new ConstraintException("Astronaut announced arrival without being on their way.");
    }

    public override void AnnounceDeath() {
        
        if (myBody.alive) {
            throw new ConstraintException("Astronaut announced death without dying first.");
        }
        
        //If ya dead, ya dead
        myStats.myState = AstroStats.AIStates.Dead;
            
        //Become unassigned from all stations
        myStats.myBehaveStation.Leave(this);
        myStats.myMisbehaveStation.Leave(this);
    }

}
