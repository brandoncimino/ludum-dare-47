using System;

using DefaultNamespace;
using DefaultNamespace.Text;

using UnityEngine;

using Random = System.Random;

public class MisbehaveStation : ActivityStation {
    // public float OffsetAngle = -12;
    public    Sprite BridgeSpriteCat;
    protected Random Random = new Random();

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

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    void Awake() {
        remainingBreakTime = maxTimeToBreak;
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
            currentState       = MisbehaveStationStates.Fixed;

            if (DoorSign == ActivityRoom.Lab) {
                behaveTwin.MonsterInStorage = true;
            }
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
            case MisbehaveStationStates.Broken:
                //Astronauts that are still at a location when it's broken slowly die
                return isLethal ? deltaTime : 0f;
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
                astronaut.myBody.ChangeThought(Thought.Pyromania);
            }

            if (DoorSign == ActivityRoom.Bridge) {
                mySpriteRenderer.sprite = BridgeSpriteCat;
            }

            Assignees.Add(astronaut);

            // send an alert message for the arrival at this station.
            //TODO: This would ideally use an event system, either with "real" events or UnityEvents, but that might require more refactoring than is worthwhile.
            //AlertArrival(this, astronaut);

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
                return 2 * Assignees.Count;
            case ActivityRoom.Rec:
                // misbehaviour in the rec room causes damage to the window
                return BreakWindow(timePassed);
            case ActivityRoom.Kitchen:
                // misbehaviour in the kitchen sets fire which hurts the astronauts
                return SetFire(timePassed);
            case ActivityRoom.Lab:
                // misbehaviour in the lab frees the Monster
                return FreeMonster(timePassed);
            case ActivityRoom.Engine:
                // misbehaviour in the engine room causes double acceleration
                return 2 * Assignees.Count;
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

            return 0;
        }

        // an intact window doesn't cause acceleration
        // TODO: maybe a percentage?
        return 0;
    }

    private float SetFire(float timePassed) {
        // returns acceleration caused in the process

        // fire in the kitchen harms a random astronaut at the arson spot
        // the random choice prevents an error where an astronaut dies from fire damage, gets unassigned from the
        // arson spot, which changes the Assignees list and hence breaks the for-loop over its items
        if (Random.Next(0, 100) == 0) {
            var astronaut = Assignees[Random.Next(0, Assignees.Count)];
            var nonLethal = astronaut.myBody.TakeDamage(0.5f, 1.5f * astronaut.myBody.getDmgVisualTime());

            if (!nonLethal) {
                Scheduler.Single.ReportEmergency(StationAlertType.Astronaut_Dead_Fire, astronaut);
            }
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
        if (behaveTwin.MonsterInStorage && remainingBreakTime <= 0) {
            // consider this station broken
            currentState       = MisbehaveStationStates.Broken;
            remainingBreakTime = 0;
            home.JailBreak(PositionAngle);
            behaveTwin.MonsterInStorage = false;
        }

        // freeing a monster doesn't cause acceleration
        return 0;
    }

    public override void GiveWarning() {
        behaveTwin.GiveWarning();
    }

    public override void GiveUpdate() {
        behaveTwin.GiveUpdate();
    }
}