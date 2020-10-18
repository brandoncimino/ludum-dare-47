using System;

using DefaultNamespace;

public class MisbehaveStation : ActivityStation {
    // public float OffsetAngle = -12;

    protected Random Random = new Random();

    public BehaveStation behaveTwin;

    //Time in seconds it takes to break;
    public float maxTimeToBreak = 8f;
    public float remainingBreakTime;

    public MisbehaveStationStates currentState = MisbehaveStationStates.Fixed;

    void Start() {
        remainingBreakTime = maxTimeToBreak;
        AstroForeman.Single.Register(this);
    }

    private void Update() {
        // TODO: change sprites
        // TODO: update health bars
    }

    public virtual void Repair(float deltaTime) {
        remainingBreakTime += deltaTime;
        if (remainingBreakTime >= maxTimeToBreak) {
            remainingBreakTime = maxTimeToBreak;
            currentState       = MisbehaveStationStates.Fixed;
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
            Assignees.Remove(astronaut);
            return true;
        }

        return false;
    }

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
}