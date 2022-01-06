using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseMission : ScriptableObject
{
    public string missionName;
    public string missionDescription;
    public bool missionCompleted;

    [Header("Rewards")]
    //Add specific faction here
    public float influenceXPReward;
    public List<BaseItem> itemRewards;

    [Space(5)]

    public List<BaseMission> missionRequirements;

    public abstract void Complete();

    public abstract void Progress();
}
