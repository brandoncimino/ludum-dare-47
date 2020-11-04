using System;

using DefaultNamespace;

using UnityEngine;

using Random = System.Random;

public class Creature : MonoBehaviour {
    public    SpriteRenderer mySpriteRenderer;
    public    CreatureAI     myBrain;
    protected Random         rando = new Random();

    // movement and positional information

    #region Movement & Position

    public    float targetAngle   = 90;
    public    float positionAngle = 0;
    public    float speedAngle    = 1;
    public    int   layer         = 0;
    protected bool  hasArrived    = false;
    public    float speedMod      = 1f;

    #endregion


    #region Fidgeting

    // rotation
    protected float rFidgetAngle       = 0;
    private   float rMaxFidgetAngle    = 15f;
    private   float rMinFidgetAngle    = 2f; // needs to be non-zero
    private   float rFidgetSpeedAngle  = 28f;
    private   float rFidgetTargetAngle = 10f; // needs to be initialized as non-zero

    // translation
    protected int  tSidesteps    = 25;
    protected int  tMaxSidesteps = 50;
    protected bool fidgetRight   = false;

    #endregion

    // information about the space station
    // TODO: call SpaceStation class instead
    public SpaceStation home;

    // damage and stuff
    public const float maxHitPoints = 5f;
    public       float currentHitPoints;
    public       bool  alive                   = true;
    protected    float dmgVisualizationTime    = 0;
    protected    float maxDmgVisualizationTime = 0.25f;
    protected    Color suitcolor;
    protected    bool  needColorChange = false;
    protected    float dmgImmunityTime = 0;


    // Start is called before the first frame update
    void Start() {
        // randomly assign a color
        mySpriteRenderer.color = new Color(
            rando.Next(0, 255) / 255f,
            rando.Next(0, 255) / 255f,
            rando.Next(0, 255) / 255f
        );
        suitcolor = mySpriteRenderer.color;

        // resize to the correct size (relative to parent space station)
        transform.localScale = new Vector3(5.243802f, 5.243802f, 5.243802f);

        // set hit points
        currentHitPoints = maxHitPoints;

        // give home
        home = SpaceStation.Single;
    }

    // Update is called once per frame
    void Update() {
        // apply movement rule
        if (alive) {
            MovementRule();
        }

        // check for end-of-damage-color time
        dmgVisualizationTime = Math.Max(dmgVisualizationTime - Time.deltaTime, 0);
        if (needColorChange && dmgVisualizationTime == 0) {
            mySpriteRenderer.color = suitcolor;
            needColorChange        = false;
        }

        // damage immunity wears off
        dmgImmunityTime = Math.Max(dmgImmunityTime - Time.deltaTime, 0);

        // individual update methods from subclasses
        IndividualUpdate();
    }

    protected virtual void MovementRule() {
        if (hasArrived) {
            Fidget();
        }
        else {
            // the creature moves toward its target location
            MoveTowardTarget();
        }
    }

    protected virtual void IndividualUpdate() {
        // not in the base class
    }

    public void ChangeLayer(int newLayer) {
        layer = newLayer;
    }

    protected void MoveTowardTarget() {
        // determine angular change based on speed and passed time
        var angularDistance = speedAngle * Time.deltaTime * speedMod;

        if (Math.Abs((targetAngle - positionAngle + 360) % 360) < angularDistance) {
            // if you have more movement than distance, stop at the target position
            positionAngle = targetAngle;
            hasArrived    = true;
            myBrain.AnnounceArrival();
        }
        else {
            // determine movement direction based on smallest angle and new position angle
            if (Math.Abs(targetAngle - positionAngle) < 180) {
                positionAngle = (positionAngle + Math.Sign(targetAngle - positionAngle) * angularDistance) % 360;
                mySpriteRenderer.flipX = Math.Sign(targetAngle - positionAngle) <= 0;
            }
            else {
                positionAngle = (positionAngle - Math.Sign(targetAngle - positionAngle) * angularDistance + 360) % 360;
                mySpriteRenderer.flipX = Math.Sign(targetAngle - positionAngle) >= 0;
            }
        }

        // move creature sprite to positional angle
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            -(home.depth / 2) + layer * home.depth / home.noLayers,
            (float) (home.radius * Math.Sin(angle))
        );

        // adjust rotation
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    //[EditorInvocationButton]
    public void MoveClockwise(float mod = 1f) {
        positionAngle = (positionAngle - speedAngle * Time.deltaTime * mod + 360) % 360;
        var angle = Math.PI * (positionAngle) / 180;

        Transform transform1;
        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            -(home.depth / 2) + layer * home.depth / home.noLayers,
            (float) (home.radius * Math.Sin(angle))
        );

        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    //[EditorInvocationButton]
    public void MoveCounterClockwise(float mod = 1f) {
        positionAngle = (positionAngle + speedAngle * Time.deltaTime * mod) % 360;
        var       angle = Math.PI * (positionAngle) / 180;
        Transform transform1;

        (transform1 = transform).localPosition = new Vector3(
            (float) (home.radius * Math.Cos(angle)),
            -(home.depth / 2) + layer * home.depth / home.noLayers,
            (float) (home.radius * Math.Sin(angle))
        );

        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    protected void Fidget() {
        // TRANSLATION
        // translation must happen before rotation since MoveClockwise and MoveCounterClockwise reset rotation
        // 20% change for taking a step sideways
        if (rando.NextDouble() < 0.2) {
            // if stepped far enough sideways, switch stepping direction
            if (tSidesteps == tMaxSidesteps) {
                fidgetRight = !fidgetRight;
                tSidesteps  = 0;
            }

            // take a step
            tSidesteps++;
            if (fidgetRight) {
                MoveClockwise();
            }
            else {
                MoveCounterClockwise();
            }
        }

        // ROTATION
        // if you are there already, find new target angle for fidgeting
        if (Math.Abs(rFidgetAngle - rFidgetTargetAngle) < 1e-6) {
            // random angle between minFidgetAngle and maxFidgetAngle (uniform distribution), but at the other side of 0 than the previous fidgetTargetAngle
            // attention: Math.Sign(0) = 0. Hence, if fidgetTargetAngle == 0 it will stay as 0. At the moment this can only happen if minFidgetAngle == 0.
            rFidgetTargetAngle = -(rMinFidgetAngle + (rMaxFidgetAngle - rMinFidgetAngle) * (float) rando.NextDouble()) *
                                 Math.Sign(rFidgetTargetAngle);
        }

        // change angle toward target angle, remain within limits
        // factor (0.5 + rando.NextDouble()) is so that fiddling speed is not uniform
        rFidgetAngle += Time.deltaTime * rFidgetSpeedAngle * Math.Sign(rFidgetTargetAngle - rFidgetAngle) *
                        (float) (0.5 + rando.NextDouble());
        rFidgetAngle = Math.Max(-Math.Abs(rFidgetTargetAngle), Math.Min(Math.Abs(rFidgetTargetAngle), rFidgetAngle));

        // rotate to new fidgeting angle
        transform.localEulerAngles = new Vector3(90, 0, positionAngle + rFidgetAngle + 90);
    }

    public void SetTarget(float newAngle) {
        targetAngle  = Math.Abs(newAngle) % 360f;
        hasArrived   = false;
        speedMod     = 0.75f + 0.5f * (float) rando.NextDouble(); // random speed modifier until next arrival
        rFidgetAngle = 0;
        tSidesteps   = tMaxSidesteps / 2;
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
        return Math.Min(Math.Abs(angle - positionAngle), 360f - Math.Abs(angle - positionAngle));
    }

    public static float Distance2AngleAsAngle(float angle1, float angle2) {
        return Math.Min(Math.Abs(angle1 - angle2), 360f - Math.Abs(angle1 - angle2));
    }

    public float Distance2Angle(float angle) {
        return home.radius * ((float) Math.PI) * Distance2AngleAsAngle(angle) / 180f;
    }

    public virtual void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            if (!needColorChange) {
                // briefly change color to show that click has been registered
                // return to original color because random color changes are above people's understanding
                dmgVisualizationTime   = maxDmgVisualizationTime;
                mySpriteRenderer.color = Color.green;
                needColorChange        = true;
                Heal(0.5f);
            }
        }
    }


    public virtual void Kill() {
        alive = false;
        myBrain.AnnounceDeath();

        // no more moving around when you are dead! (for now)
        speedAngle = 0;

        // let's make you a skeleton
        mySpriteRenderer.color = Color.white;
        // TODO: Sprite for dead creature

        // collapse to the ground
        transform.localEulerAngles = new Vector3(90, 0, positionAngle);
    }

    public bool TakeDamage(float dmg, float maxDmgImmunityTime = 0) {
        if (dmgImmunityTime == 0) {
            // deliver the damage
            currentHitPoints -= dmg;
            dmgImmunityTime  =  maxDmgImmunityTime;

            // visualization
            dmgVisualizationTime   = maxDmgVisualizationTime;
            mySpriteRenderer.color = Color.red;
            needColorChange        = true;

            // check for lethal
            if (currentHitPoints <= 0) {
                Kill();
            }
        }

        return alive;
    }

    public void Heal(float increase = maxHitPoints) {
        currentHitPoints = Math.Min(currentHitPoints + increase, maxHitPoints);
        //mySpriteRenderer.color = suitcolor;
        //needColorChange        = false;
    }

    public float getLifePercentage() {
        return currentHitPoints / maxHitPoints;
    }

    public float getDmgVisualTime() {
        return maxDmgVisualizationTime;
    }

    public void setSuitcolor(Color newColor) {
        suitcolor              = newColor;
        mySpriteRenderer.color = suitcolor;
    }
}