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

    #endregion


    #region Fidgeting

    protected float fidgetAngle       = 0;
    private   float maxFidgetAngle    = 15f;
    private   float minFidgetAngle    = 2f; // needs to be non-zero
    private   float fidgetSpeedAngle  = 30f;
    private   float fidgetTargetAngle = 10f; // needs to be initialized as non-zero

    #endregion

    // information about the space station
    // TODO: call SpaceStation class instead
    public SpaceStation home;

    // damage and stuff
    public    float maxHitPoints = 5f;
    public    float currentHitPoints;
    public    bool  alive                   = true;
    protected float dmgVisualizationTime    = 0;
    protected float maxDmgVisualizationTime = 0.25f;
    protected Color suitcolor;
    protected bool  needColorChange = false;
    protected float dmgImmunityTime = 0;


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
        if (Math.Abs((targetAngle - positionAngle + 360) % 360) < speedAngle * Time.deltaTime) {
            positionAngle = targetAngle;
            hasArrived    = true;
            myBrain.AnnounceArrival();
        }
        else {
            if (Math.Abs(targetAngle - positionAngle) < 180) {
                positionAngle = (positionAngle + Math.Sign(targetAngle - positionAngle) * speedAngle * Time.deltaTime) %
                                360;
                mySpriteRenderer.flipX = Math.Sign(targetAngle - positionAngle) <= 0;
            }
            else {
                positionAngle = (positionAngle - Math.Sign(targetAngle - positionAngle) * speedAngle * Time.deltaTime +
                                 360) % 360;
                mySpriteRenderer.flipX = Math.Sign(targetAngle - positionAngle) >= 0;
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

        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
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

        // transform.Rotate(new Vector3(1, 0, 1), speedAngle);
        transform1.localEulerAngles = new Vector3(90, 0, positionAngle + 90);
    }

    protected void Fidget() {
        // if you are there already, find new target angle for fidgeting
        if (Math.Abs(fidgetAngle - fidgetTargetAngle) < 1e-6) {
            // random angle between minFidgetAngle and maxFidgetAngle (uniform distribution), but at the other side of 0 than the previous fidgetTargetAngle
            // attention: Math.Sign(0) = 0. Hence, if fidgetTargetAngle == 0 it will stay as 0. At the moment this can only happen if minFidgetAngle == 0.
            fidgetTargetAngle = -(minFidgetAngle + (maxFidgetAngle - minFidgetAngle) * (float) rando.NextDouble()) *
                                Math.Sign(fidgetTargetAngle);
        }

        // change angle toward target angle, remain within limits
        fidgetAngle += Time.deltaTime * fidgetSpeedAngle * Math.Sign(fidgetTargetAngle - fidgetAngle);
        fidgetAngle =  Math.Max(-Math.Abs(fidgetTargetAngle), Math.Min(Math.Abs(fidgetTargetAngle), fidgetAngle));

        // rotate to new fidgeting angle
        transform.localEulerAngles = new Vector3(90, 0, positionAngle + fidgetAngle + 90);
    }

    public void SetTarget(float newAngle) {
        targetAngle = Math.Abs(newAngle) % 360f;
        hasArrived  = false;
        fidgetAngle = 0;
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
            // briefly change color to show that click has been registered
            // return to original color because random color changes are above people's understanding
            dmgVisualizationTime   = maxDmgVisualizationTime;
            mySpriteRenderer.color = Color.white;
            needColorChange        = true;
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

    public void Heal() {
        currentHitPoints       = maxHitPoints;
        mySpriteRenderer.color = suitcolor;
        needColorChange        = false;
    }

    public float getDmgVisualTime() {
        return maxDmgVisualizationTime;
    }

    public void setSuitcolor(Color newColor) {
        suitcolor              = newColor;
        mySpriteRenderer.color = suitcolor;
    }
}