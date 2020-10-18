using System.Collections.Generic;
using System.Linq;

using DefaultNamespace;

using JetBrains.Annotations;

using Packages.BrandonUtils.Runtime.Collections;

using UnityEngine;

public class AstroForeman : MonoBehaviour {
    public List<AstroAI>          StationedAstronauts = new List<AstroAI>();
    public List<BehaveStation>    BehaveStations      = new List<BehaveStation>();
    public List<MisbehaveStation> MisbehaveStations   = new List<MisbehaveStation>();
    //ASSUMES that BehaveStation[X] is in the same room as MisbehaveStation[X]

    public static AstroForeman Single;

    private void Awake() {
        if (Single == null) {
            Single = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    public void Register(BehaveStation applyingStation) {
        BehaveStations.Add(applyingStation);
    }

    public void Register(MisbehaveStation applyingStation) {
        MisbehaveStations.Add(applyingStation);
    }

    public MisbehaveStation AssignMisbehavior(
        [CanBeNull]
        BehaveStation myBehaveStation = null
    ) {
        //Return random Misbehavior Station
        return myBehaveStation is null ? MisbehaveStations.Random() : DistantMisbehaveStation(myBehaveStation);
    }

    public BehaveStation AvailableBehaveStation() {
        return BehaveStations.Where(station => station.currentState == BehaveStationStates.Abandoned).Random();
    }

    public MisbehaveStation DistantMisbehaveStation(BehaveStation currentLocation) {
        return MisbehaveStations.Where(station => station.behaveTwin != currentLocation).Random();
    }

    public bool AssignBehavior(AstroAI astronaut) {
        //Sort stations by distance
        BehaveStations = BehaveStations.OrderBy(station => astronaut.myBody.Distance2Angle(station.PositionAngle))
                                       .ToList();
        //Find closest abandoned station
        foreach (var station in BehaveStations) {
            if (station.currentState == BehaveStationStates.Abandoned) {
                //set astronaut's station to that station
                station.Claim(astronaut);
                astronaut.myStats.myBehaveStation = station;
                return true;
            }
        }

        // no free behave station was found
        return false;
    }

    //Negative Modulus is stupid. This adds the divider back into the answer if the answer is negative to create the 
    // correct positive modulus.
    public int MathLoop(int input, int maxAllowed) {
        return (input % maxAllowed + maxAllowed) % maxAllowed;
    }
}