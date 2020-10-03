using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AstronautVisuals : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    public GameObject myStation;
    Random rando = new Random();

    // degree of speed, as an element between 0 and 360 degree, for the moment as int
    public float speedAngle = 20;
    
    // angle that describes where I am on the circle that is my space station
    public float positionAngle = 0;
    
    // radius of the space station
    public double radiusSpacestation = 14.1033;
    
    // target location
    public float   targetAngle = 90;
    
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer.color = new Color(rando.Next(0,255),rando.Next(0,255),rando.Next(0,255));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            MoveTowardTarget();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveClockwise();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveCounterClockwise();
        }

    }

    void MoveTowardTarget() {
        if (Math.Abs(targetAngle - positionAngle) < 180) {
            positionAngle = (positionAngle + Math.Sign(targetAngle-positionAngle) * speedAngle) % 360;
        }
        else {
            positionAngle = (positionAngle - Math.Sign(targetAngle-positionAngle) * speedAngle + 360) % 360;
        }
            
        var angle = Math.PI * (positionAngle) / 180;
        transform.localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }
    
    
    void MoveClockwise() {
        positionAngle = (positionAngle - speedAngle + 360) % 360;
        var angle = Math.PI * (positionAngle) / 180;
        transform.localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }
    
    void MoveCounterClockwise() {
        positionAngle = (positionAngle + speedAngle) % 360;
        var angle = Math.PI * (positionAngle) / 180;
        transform.localPosition = new Vector3(
            (float) (radiusSpacestation * Math.Cos(angle)),
            0,
            (float) (radiusSpacestation * Math.Sin(angle))
        );
            
        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform.localEulerAngles = new Vector3(90, 0, positionAngle+90);
    }
    
}
