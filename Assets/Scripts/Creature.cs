using System;
using System.Collections;
using System.Collections.Generic;

using Packages.BrandonUtils.Editor;

using UnityEngine;
using Random = System.Random;

public class Creature : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    public GameObject myStation;
    Random rando = new Random();

    // degree of speed, as an element between 0 and 360 degree, for the moment as int
    public float speedAngle = 1;
    
    // angle that describes where I am on the circle that is my space station
    public float positionAngle = 0;
    
    // radius of the space station
    public float radiusSpacestation = 14.1033f;
    
    // target location
    public float   targetAngle = 90;
    
    // Start is called before the first frame update
    void Start()
    {
        // randomly assign a color
        mySpriteRenderer.color = new Color(rando.Next(0,255)/255f,rando.Next(0,255)/255f,rando.Next(0,255)/255f);
    }

    // Update is called once per frame
    void Update()
    {
        // the creature moves toward its target location
        if (Distance2TargetAsAngle() > speedAngle) {
            MoveTowardTarget();
        }
        
    }

    public void MoveTowardTarget() {

        if (Math.Abs(targetAngle - positionAngle) < 180) {
            positionAngle = (positionAngle + Math.Sign(targetAngle-positionAngle) * speedAngle) % 360;
        }
        else {
            positionAngle = (positionAngle - Math.Sign(targetAngle-positionAngle) * speedAngle + 360) % 360;
        }
            
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }
    
    [EditorInvocationButton]
    public void MoveClockwise() {
        positionAngle = (positionAngle - speedAngle + 360) % 360;
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }
    
    [EditorInvocationButton]
    public void MoveCounterClockwise() {
        positionAngle = (positionAngle + speedAngle) % 360;
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        
        (transform1 = transform).localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }

    public void SetTarget(float newAngle) {
        targetAngle = Math.Abs(newAngle) % 360f;
    }

    public void SetTarget(Vector3 location) {
        // location is in the global coordinate system
        // TODO: once we have a space station class that knows about its rotation, this projection needs to be corrected
        // right now this function assumes that the station is rotated so that its normal is the z-direction
        
        // get difference to center of the station
        var diff = location - transform.parent.position;
        
        // project into 2-dim space
        diff.z = 0;
        
        // project onto unit circle
        if (diff.magnitude < 1e-14) {
            // make sure we don't divide by zero
            throw new ArithmeticException("In SetTarget: Projection onto spaceship-plane ended too close to the center");
        }
        diff /= diff.magnitude;
        
        // get angle as radian
        var angle = Math.Acos(diff.x);
        if (diff.y < 0) {
            angle += Math.PI;
        }
        
        // transform to degree
        targetAngle = (float) ((float) angle * 180 / Math.PI);
    }

    public float Distance2TargetAsAngle() {
        return Math.Min(Math.Abs(targetAngle - positionAngle), 360f - Math.Abs(targetAngle - positionAngle));
    }

    public float Distance2Target() {
        return radiusSpacestation * ((float) Math.PI) * Distance2TargetAsAngle() / 180f;
    }
}
