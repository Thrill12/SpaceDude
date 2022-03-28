using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Faction> factions = new List<Faction>();
}

public class Faction : ScriptableObject
{
    public string factionName;
    public Sprite factionIcon;
    public int factionReputation;
}