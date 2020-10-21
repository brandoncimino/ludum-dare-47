using System;

using DefaultNamespace;
using DefaultNamespace.Text;

public class MisbehaveStation : ActivityStation {
    // public float OffsetAngle = -12;

    protected Random Random = new Random();

    public BehaveStation behaveTwin;

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    private void Start() {
        remainingBreakTime = maxTimeToBreak;
        AstroForeman.Single.Register(this);
    }

    private void Update() {
        // TODO: change sprites
        // TODO: update health bars
    }

    /// <summary>
    /// repair this station. This increases the time it takes until it breaks and unleashes its "broken" behaviour (e.g. the monster).
    /// While a station is not fixed, any worker at the behave twin prioritizes repairs over deceleration and other positive consequences of their good behaviour.
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void Repair(float deltaTime) {
        remainingBreakTime += deltaTime;
        if (remainingBreakTime >= maxTimeToBreak) {
            remainingBreakTime = maxTimeToBreak;
            currentState       = MisbehaveStationStates.Fixed;
        }
    }

    protected override bool IsBehaveStation() {
        return false;
    }

    public override bool Arrive(AstroAI astronaut) {
        if (!Assignees.Contains(astronaut)) {
            Assignees.Add(astronaut);
            return Arrive_individual(astronaut);
        }

        return false;
    }

    protected virtual bool Arrive_individual(AstroAI astronaut) {
        return true;
    }

    public override bool Leave(AstroAI astronaut) {
        if (Assignees.Contains(astronaut)) {
            Assignees.Remove(astronaut);
            return true;
        }

        return false;
    }

    /// <summary>
    /// This function tells us what happens from work at the station. It gets called from the space station with the time that has passed since the frame update. It return the acceleration (negative: deceleration) caused by the worker at this station. If the work has additional consequences than acceleration / deceleration, this is where they get called and come to life.
    /// </summary>
    /// <param name="timePassed">time passed since the last update</param>
    /// <returns>acceleration caused by this station (not weighted by the time)</returns>
    public override float DetermineConsequences(float timePassed) {
        // returns average acceleration over last time interval
        // this function gets overriden in the subclasses with specific negative consequences, but as a generic version for new misbehave stations in the future, Assignees.Count is a first good value.
        return Assignees.Count;
    }

    public override void GiveWarning() {
        behaveTwin.GiveWarning();
    }

    public override void GiveUpdate() {
        behaveTwin.GiveUpdate();
    }

    public override StationAlertType AstronautInfo() {
        // as a generic version, we give the behaviour message of the station instead of what an astronaut might write
        return GetAlert(DoorSign, false);
    }
}