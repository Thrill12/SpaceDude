using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public List<Faction> factions = new List<Faction>();

    public void AddReputation(string factionName, int reputation)
    {
        Faction factionToChange = factions.First(x => x.factionName == factionName);

        factionToChange.AddXP(reputation);
    }

    public bool CheckReputation(string factionName, int level)
    {
        Faction factionToCheck = factions.First(x => x.factionName == factionName);

        if(factionToCheck.factionLevel >= level)
        {
            Debug.Log("Have enough for check");
            return true;
        }
        else
        {
            Debug.Log("Not enough for check");
            return false;
        }
    }
}