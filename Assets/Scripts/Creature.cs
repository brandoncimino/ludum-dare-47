using System;

using DefaultNamespace;

using Packages.BrandonUtils.Editor;

using UnityEngine;

using Random = System.Random;

public class Creature : MonoBehaviour {
    public SpriteRenderer mySpriteRenderer;
    public GameObject     myStation;
    Random                rando = new Random();

    // movement and positional information

    #region Movement & Position

    public float positionAngle = 0;
    public float speedAngle    = 1;
    public int   layer         = 0;

    #endregion

    // target location
    public float targetAngle = 90;

    // information about the space station
    // TODO: call SpaceStation class instead
    public SpaceStation home;


    // Start is called before the first frame update
    void Start() {
        // randomly assign a color
        mySpriteRenderer.color = new Color(
            rando.Next(0, 255) / 255f,
            rando.Next(0, 255) / 255f,
            rando.Next(0, 255) / 255f
        );

        // resize to the correct size (relative to parent space station)
        transform.localScale = new Vector3(5.243802f, 5.243802f, 5.243802f);
    }

    // Update is called once per frame
    void Update() {
        // the creature moves toward its target location
        MoveTowardTarget();
    }

    public void ChangeLayer(int newLayer) {
        layer = newLayer;
    }

    public void GiveHome(SpaceStation newHome) {
        home = newHome;
    }

    private void MoveTowardTarget() {
        // TODO: change direction into which the astronaut looks

        if (Math.Abs((targetAngle - positionAngle + 360) % 360) < speedAngle * Time.deltaTime) {
            positionAngle = targetAngle;
        }
        else {
            if (Math.Abs(targetAngle - positionAngle) < 180) {
                positionAngle = (positionAngle + Math.Sign(targetAngle - positionAngle) * speedAngle * Time.deltaTime) %
                                360;
                mySpriteRenderer.flipX = true;
            }
            else {
                positionAngle =
                    (positionAngle - Math.Sign(targetAngle - positionAngle) * speedAngle * Time.deltaTime + 360) % 360;
                mySpriteRenderer.flipX = false;
            }
        }

        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            -(home.depth / 2) + layer * home.depth / home.noLayers,
            (float) (home.radius * Math.Sin(angle))
        );

        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    [EditorInvocationButton]
    public void MoveClockwise() {
        positionAngle = (positionAngle - speedAngle + 360) % 360;
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            0,
            (float) (home.radius * Math.Sin(angle))
        );

        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    [EditorInvocationButton]
    public void MoveCounterClockwise() {
        positionAngle = (positionAngle + speedAngle) % 360;
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;

        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            0,
            (float) (home.radius * Math.Sin(angle))
        );

        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
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
            throw new ArithmeticException(
                "In SetTarget: Projection onto spaceship-plane ended too close to the center"
            );
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
        return home.radius * ((float) Math.PI) * Distance2TargetAsAngle() / 180f;
    }
    
    public float Distance2AngleAsAngle(float angle) {
        return Math.Min(Math.Abs(angle - positionAngle), 360f - Math.Abs(targetAngle - positionAngle));
    }

    public float Distance2Angle(float angle) {
        return home.radius * ((float) Math.PI) * Distance2AngleAsAngle(angle) / 180f;
    }

    public void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            // TODO: change AI behaviour

            // randomly assign a new color
            mySpriteRenderer.color = new Color(
                rando.Next(0, 255) / 255f,
                rando.Next(0, 255) / 255f,
                rando.Next(0, 255) / 255f
            );
        }
    }
}