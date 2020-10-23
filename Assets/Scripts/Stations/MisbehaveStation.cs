using DefaultNamespace;
using DefaultNamespace.Text;

using UnityEngine;

using Random = System.Random;

public class MisbehaveStation : ActivityStation {
    // public float OffsetAngle = -12;

    protected Random Random = new Random();

    public BehaveStation behaveTwin;

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    #region information to do with warnings

    protected float WarnCounter      = 0;
    protected float MaxWaitTime      = 5f;
    protected bool  reportedWarning  = false;
    protected bool  canReportWarning = true;

    #endregion

    private void Start() {
        remainingBreakTime = maxTimeToBreak;
        AstroForeman.Single.Register(this);
    }

    private void Update() {
        // see if enough time has passed since last warning to regain warning ability
        if (!canReportWarning && !reportedWarning) {
            WarnCounter -= Time.deltaTime;
            if (WarnCounter <= 0) {
                // wait time has passed, can report again
                WarnCounter      = 0;
                canReportWarning = true;
            }
        }

        // check if it is worth reporting a warning and do so if needed
        if (canReportWarning && Assignees.Count > 0 && Reason2Warn()) {
            Scheduler.Single.ReportWarning(this);
            reportedWarning  = true;
            canReportWarning = false;
        }

        // TODO: change sprites to reflect state of the station
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
            return Leave_individual(astronaut);
        }

        return false;
    }

    protected virtual bool Leave_individual(AstroAI astronaut) {
        return true;
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

    public override bool GiveWarning() {
        if (Reason2Warn()) {
            // reset warn counter
            reportedWarning = false;
            WarnCounter     = MaxWaitTime;

            // give warning
            StationLogger.Alert(GetAlert(DoorSign, false), Alert.SeverityLevel.Warning, this);

            // warning has indeed happened
            return true;
        }

        // there was no reason for a warning
        return false;
    }

    public override void GiveUpdate() {
        // actually, a misbehave station should never be called upon to give an info update
        behaveTwin.GiveUpdate();
    }

    public override StationAlertType AstronautInfo() {
        // as a generic version, we give the behaviour message of the station instead of what an astronaut might write
        return GetAlert(DoorSign, false);
    }

    protected virtual bool Reason2Warn(float threshold = 0.7f) {
        // no reason to warn by default
        return false;
    }
}