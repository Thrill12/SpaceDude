using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    public List<NPC> allNPCs = new List<NPC>();

    private void Awake()
    {
        instance = this;
        try
        {
            GameManager.instance.LoadNPCStates();
        }
        catch
        {
            Debug.Log("No save or no game manager");
        }
    }

    private void Start()
    {      
        QuestManager.instance.InitializeQuests();
    }

    public void SetNPCToGraph(string npcName, string graphPath)
    {
        allNPCs.First(x => x.npcName == npcName).dialogueGraph = Resources.Load<DialogueGraph>(graphPath);

        GameManager.instance.SaveNPCStates();
    }
}
