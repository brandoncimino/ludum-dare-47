using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

public class MisbehaveStation : ActivityStation
{
    // public float OffsetAngle = -12;
    public Sprite BridgeSpriteCat;
    public enum MisbehaveStationStates {
        Fixed,
        Damaged,
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
        AstroForeman.Single.Register(this);
        
    }

    private void Update() {

        // TODO: change sprites
        // TODO: update health bars
        
    }

    public void Repair(float deltaTime) {
        remainingBreakTime += deltaTime;
        if (remainingBreakTime >= maxTimeToBreak) {
            remainingBreakTime = maxTimeToBreak;
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
    
    protected override bool IsBehaviourStation() {
        return false;
    }
    
    public override bool Arrive(AstroAI astronaut) {
        if (!Assignees.Contains(astronaut)) {

            if (DoorSign == ActivityRoom.Kitchen) {
                astronaut.myRotationData.ChangeThought(Thought.Pyromania);
            }

            if (DoorSign == ActivityRoom.Bridge) {
                mySpriteRenderer.sprite = BridgeSpriteCat;
            }
            
            Assignees.Add(astronaut);
            return true;
        }
        else {
            return false;
        }
    }
    
    public override bool Leave(AstroAI astronaut) {
        
        if (Assignees.Contains(astronaut)) {
            
            if (DoorSign == ActivityRoom.Bridge) {
                mySpriteRenderer.sprite = BridgeSprite;
            }
            
            Assignees.Remove(astronaut);
            return true;
        }

        return false;

    }
    
    public override float DetermineConsequences(float timePassed) {
        // returns average acceleration over last time interval

        if (Assignees.Count == 0) {
            return 0;
        }
        
        switch (DoorSign) {
            case ActivityRoom.Bridge:
                // misbehaviour on the bridge causes double acceleration
                return 2*Assignees.Count;
                break;
            case ActivityRoom.Rec:
                // misbehaviour in the rec room causes damage to the window
                return BreakWindow(timePassed);
                break;
            case ActivityRoom.Kitchen:
                // misbehaviour in the kitchen sets fire which hurts the astronauts
                return SetFire(timePassed);
                break;
            case ActivityRoom.Lab:
                // misbehaviour in the lab frees the Monster
                return FreeMonster(timePassed);
                break;
            case ActivityRoom.Engine:
                // misbehaviour in the engine room causes double acceleration
                return 2*Assignees.Count;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private float BreakWindow(float timePassed) {
        // returns acceleration caused in the process
        
        // damage the window
        remainingBreakTime -= timePassed * Assignees.Count;
        currentState       =  MisbehaveStationStates.Damaged;

        // check if broken
        if (remainingBreakTime <= 0) {
            
            currentState       = MisbehaveStationStates.Broken;
            remainingBreakTime = 0;
            
            // TODO: kill all astronauts (or let them die slowly)
            
            // TODO: tell the game that it's over (unless we decide a completely broken window is no reason to end it)
            
            // a broken window causes maximum acceleration (btw)
            return float.MaxValue / 2f;
        }
        
        // an intact window doesn't cause acceleration
        // TODO: maybe a percentage?
        return 0;
    }
    
    private float SetFire(float timePassed) {
        // returns acceleration caused in the process

        // fire in the kitchen harms the astronauts
        // at the moment: only damages the astronauts at the misbehave station, not the one cooking next to it
        foreach (var astronaut in Assignees) {
            astronaut.takeDamage(timePassed);
        }
        
        // fire doesn't cause acceleration
        return 0;
    }
    
    private float FreeMonster(float timePassed) {
        // returns acceleration caused in the process
        
        // damage the incarnation tube
        remainingBreakTime -= timePassed * Assignees.Count;
        currentState       =  MisbehaveStationStates.Damaged;

        // check if broken
        if (remainingBreakTime <= 0) {
            
            currentState       = MisbehaveStationStates.Broken;
            remainingBreakTime = 0;

            // TODO: spawn monster
            // TODO: implement monster

        }
        
        // freeing a monster doesn't cause acceleration
        return 0;
    }
}
