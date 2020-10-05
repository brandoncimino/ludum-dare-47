using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DefaultNamespace;

using Packages.BrandonUtils.Runtime.Collections;

using UnityEngine;

public class AstroForeman : MonoBehaviour
{
    public List<AstroAI> StationedAstronauts = new List<AstroAI>();
    public List<BehaveStation> BehaveStations = new List<BehaveStation>();
    public List<MisbehaveStation> MisbehaveStations = new List<MisbehaveStation>();
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

    public MisbehaveStation AssignMisbehavior(GameObject thisAstronaut, BehaveStation myBehaveStation, MisbehaveStation myMisbehaveStation) {
        //Set Behavior Station to Abandoned
        //BehaveStations.FindIndex(Equals(myBehaveStation)).currentState = BehaveStationStats.BehaveStationStates.Abandoned;
        //myBehaveStation.currentState = BehaveStationStats.BehaveStationStates.Abandoned;
        myBehaveStation.Abandon();
        //Return random Misbehavior Station
        return myMisbehaveStation = DistantMisbehaveStation(myBehaveStation);
        //throw new NotImplementedException();
    }

    public BehaveStation AvailableBehaveStation() {
        return BehaveStations
               .Where(station => station.currentState == BehaveStation.BehaveStationStates.Abandoned)
               .Random();
    }

    public MisbehaveStation DistantMisbehaveStation(BehaveStation currentLocation) {
        return MisbehaveStations.Where(station => station.behaveTwin != currentLocation).Random();
    }

    public BehaveStation AssignBehavior(GameObject astronaut) {
        //Sort stations by distance
        BehaveStations = BehaveStations.OrderBy(
            station => (station.transform.localPosition - astronaut.gameObject.transform.localPosition).sqrMagnitude
        ).ToList();
        //Find closest abandoned station
        foreach (var station in BehaveStations) {
            if (station.currentState == BehaveStation.BehaveStationStates.Abandoned) {
                //set astronaut's station to that station
                station.currentState = BehaveStation.BehaveStationStates.Occupied;
                return station;
            }
        }
        throw new Exception("No abandoned stations found!");
    }

    //Negative Modulus is stupid. This adds the divider back into the answer if the answer is negative to create the 
    // correct positive modulus.
    public int MathLoop(int input, int maxAllowed) {
        return (input % maxAllowed + maxAllowed) % maxAllowed;
    }
}