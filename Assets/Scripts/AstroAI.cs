using System;
using System.Collections;
using System.Collections.Generic;

using DefaultNamespace;

using UnityEngine;

[RequireComponent(typeof(AstronautVisuals))]
public class AstroAI : MonoBehaviour
{
    // Start is called before the first frame update
    public  AstroStats       myStats;
    public  AstroForeman     Foreman;
    private AstronautVisuals myRotationData;
    
    void Start() {
        myStats.myBehaveStation = Foreman.AssignBehavior(gameObject);
        myRotationData          = gameObject.GetComponent<AstronautVisuals>();
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
                    myStats.myBehaveStation.currentState = BehaveStationStats.BehaveStationStates.Occupied;
                    myStats.myState                      = AstroStats.AIStates.Behaving;
                }
                //If too bored, start misbehaving
                BoredomCheck();
                break;
            case AstroStats.AIStates.Behaving:
                BoredomCheck();
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
                //Get more bored
                BoredomProgression();
                break;
            case AstroStats.AIStates.Behaving:
                //work it, gurl
                //Get more bored
                BoredomProgression();
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

    private void BoredomCheck() {
        if (myStats.timeUntilMisbehave <= 0f) {
            myStats.myMisbehaveStation = Foreman.AssignMisbehavior(gameObject, myStats.myBehaveStation, myStats.myMisbehaveStation);
            myStats.myState   = AstroStats.AIStates.MoveToMisbehaving;
        }
    }

    private bool IsAngularCloseEnough() {
        //return Math.Abs(myAngle-targetAngle) <= interactAngle
        return Mathf.Abs(myRotationData.positionAngle - myStats.targetAngle) <= myStats.interactAngle;
    }

    private void BoredomProgression() {
        //Probably could use different numbers
        //This would also be where gradual difficulty increase could come into effect
        myStats.timeUntilMisbehave -= Time.deltaTime;
    }

    private float GetNewTargetAngle(ActivityStation newStation) {
        //Either the new Station has the angle baked in, or use a function to find the angle
        return 20f;
    }
}
