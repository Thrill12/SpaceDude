using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationEnteredGoal : Goal
{
    public string locationToEnterName;

    public LocationEnteredGoal(Quest quest, string name, string description, string locationName)
    {
        this.goalQuest = quest;
        this.GoalName = name;
        this.GoalDescription = description;
        this.locationToEnterName = locationName;
    }

    public override void Init()
    {
        base.Init();
        GameEvents.instance.onLocationEntered += EnteredLocation;
    }

    public void EnteredLocation(Location location)
    {
        if(location.locationName == locationToEnterName)
        {
            Complete();
        }
    }
}
