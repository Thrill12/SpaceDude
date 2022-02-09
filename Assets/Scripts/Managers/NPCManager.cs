using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    public Dictionary<NPC, DialogueGraph> allNPCs = new Dictionary<NPC, DialogueGraph>();

    private void Awake()
    {
        instance = this;
        allNPCs = GameManager.instance.progressSave.npcStates;
    }    

    public void SetNPCToGraph(string npcName, DialogueGraph graphToChangeTo)
    {
        allNPCs.First(x => x.Key.npcName == npcName).Key.dialogueGraph = graphToChangeTo;
        allNPCs[allNPCs.First(x => x.Key.npcName == npcName).Key] = graphToChangeTo;
    }
}
