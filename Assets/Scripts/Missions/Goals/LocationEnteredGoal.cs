using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Goals/Location Goal"), System.Serializable]
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

    public override void Init(Quest quest)
    {
        base.Init(quest);
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
