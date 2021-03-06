﻿using System;
using System.Collections.Generic;
using System.Data;

using DefaultNamespace;
using DefaultNamespace.Text;

using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(Astronaut))]
public class AstroAI : CreatureAI {
    public          AstroStats myStats;
    public          Astronaut  myBody;
    private         float      remainingFleeTime = 0;
    private         float      maxFleeTime       = 5.0f;
    private         float      timeBeforeDelete  = 5f;
    public override string     DisplayName => "\"Somebody\"";
    // TODO: fix bug where when "DisplayName => myStats.myName" we get the error that myStats.myName is not set to an instance of an object after the astronaut has been freshly spawned.

    private void Start() {
        // Start is called before the first frame update
        myStats                    = gameObject.GetComponent<AstroStats>();
        myBody                     = gameObject.GetComponent<Astronaut>();
        myStats.myMisbehaveStation = AstroForeman.Single.AssignMisbehavior();
        ConvertingToGood();
    }

    // Update is called once per frame
    private void Update() {
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
            case AstroStats.AIStates.Fleeing:
                remainingFleeTime = Math.Max(remainingFleeTime - Time.deltaTime, 0);
                if (remainingFleeTime == 0) {
                    StopFleeing();
                }

                break;
            case AstroStats.AIStates.Idle:
                //Get more bored: a bit faster because idle
                MisbehaveProgression(1.2f);
                //If too bored, start misbehaving
                MisbehaveCheck();
                break;
            case AstroStats.AIStates.Dead:
                //There is no afterlife to do tasks.
                timeBeforeDelete -= Time.deltaTime;
                if (timeBeforeDelete < 0) {
                    myBody.home.Tragedy(myBody);
                    Destroy(this.gameObject);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MisbehaveCheck() {
        if (myStats.timeUntilMisbehave <= 0f) {
            // stopping old behaviour
            if (myStats.myState != AstroStats.AIStates.Idle) {
                myStats.myBehaveStation.Leave(this);
            }

            // finding new behaviour
            myStats.myMisbehaveStation = AstroForeman.Single.AssignMisbehavior(myStats.myBehaveStation);
            myStats.myState            = AstroStats.AIStates.MoveToMisbehaving;

            // new target for movement
            myBody.SetTarget(myStats.myMisbehaveStation.PositionAngle + Random.Range(-5, 5));
            // the random float is added so that the astronauts form a group rather than a queue in front of the misbehave station

            // what do you think about that?
            myBody.ChangeThought(Thought.Mischief);

            // send a message about boredom
            // StationLogger.Alert(StationAlertType.Astronaut_Bored, Alert.SeverityLevel.Warning, this);
            // N: I commented out the message about boredom because it happens too often and takes away from the importance of warnings.
        }
    }

    private void MisbehaveProgression(float multiplier = 1f) {
        //This would also be where gradual difficulty increase could come into effect
        myStats.timeUntilMisbehave -= (Time.deltaTime * multiplier);
    }

    private bool IsAngularCloseEnough() {
        return myBody.Distance2TargetAsAngle() <= myStats.interactAngle;
    }

    private void GetNewTargetAngle(ActivityStation newStation) {
        //Either the new Station has the angle baked in, or use a function to find the angle
        myBody.SetTarget(newStation.PositionAngle);
    }

    private void ConvertingToGood() {
        // show a thought adequate for working like a cat
        myBody.ChangeThought(Thought.Memes);

        // leave the misbehave station if there
        myStats.myMisbehaveStation.Leave(this);

        // find new good behaviour
        if (AstroForeman.Single.AssignBehavior(this)) {
            // free behave station was found
            GetNewTargetAngle(myStats.myBehaveStation);
            myStats.myState            = AstroStats.AIStates.MoveToBehaving;
            myStats.timeUntilMisbehave = Random.Range(15f, 23f);
        }
        else {
            // no free behave station was found
            myBody.SetTarget(myBody.positionAngle + Random.Range(-50f, 50f));
            myStats.myState            = AstroStats.AIStates.Idle;
            myStats.timeUntilMisbehave = Random.Range(15f, 23f);
        }
    }

    private void OnMouseDown() {
        if (myStats.myState == AstroStats.AIStates.Misbehaving ||
            myStats.myState == AstroStats.AIStates.MoveToMisbehaving) {
            ConvertingToGood();
        }
    }

    public override void StartFleeing(bool fleeRight = true) {
        myStats.myState     = AstroStats.AIStates.Fleeing;
        myBody.FleeingRight = fleeRight;
        remainingFleeTime   = maxFleeTime;
        myBody.ChangeThought(Thought.Alarm);
        myBody.fleeing = true;

        //Become unassigned from all stations
        myStats.myBehaveStation.Leave(this);
        myStats.myMisbehaveStation.Leave(this);
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

        if (myStats.myState == AstroStats.AIStates.Idle) {
            if (AstroForeman.Single.AssignBehavior(this)) {
                // free behave station was found
                GetNewTargetAngle(myStats.myBehaveStation);
                myStats.myState = AstroStats.AIStates.MoveToBehaving;
                return;
            }
            else {
                // no free behave station was found
                myBody.SetTarget(Random.Range(0f, 360f));
                return;
            }
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

    public override void GiveUpdate() {
        var myAlert = new List<StationAlertType>() {
            StationAlertType.Console_Line_Astronaut
        };
        var replacements = new List<IAlertReplacements>() {
            this
        };

        switch (myStats.myState) {
            case AstroStats.AIStates.MoveToBehaving:
                myAlert.Add(StationAlertType.Astronaut_Moving_Behaving);
                replacements.Add(myStats.myBehaveStation);
                break;
            case AstroStats.AIStates.Behaving:
                myAlert.Add(myStats.myBehaveStation.AstronautInfo());
                replacements.Add(myStats.myBehaveStation);
                break;
            case AstroStats.AIStates.MoveToMisbehaving:
                myAlert.Add(StationAlertType.Astronaut_Moving_Misbehaving);
                replacements.Add(myStats.myMisbehaveStation);
                break;
            case AstroStats.AIStates.Misbehaving:
                myAlert.Add(myStats.myMisbehaveStation.AstronautInfo());
                replacements.Add(myStats.myMisbehaveStation);
                break;
            case AstroStats.AIStates.Fleeing:
                myAlert.Add(StationAlertType.Astronaut_Fleeing);
                break;
            case AstroStats.AIStates.Idle:
                myAlert.Add(StationAlertType.Astronaut_Idle);
                break;
            case AstroStats.AIStates.Dead:
                myAlert.Add(StationAlertType.Astronaut_Dead_Generic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        StationLogger.Alert(myAlert, Alert.SeverityLevel.Info, replacements.ToArray());
    }
}