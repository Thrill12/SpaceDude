using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<NPC> allNPCs = new List<NPC>();

    public void SetNPCToGraph(string npcName, DialogueGraph graphToChangeTo)
    {
        allNPCs.First(x => x.npcName == npcName).GetComponent<DialogueParser>().graph = graphToChangeTo;
    }
}
