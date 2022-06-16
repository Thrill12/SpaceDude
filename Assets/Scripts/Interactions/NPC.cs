using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DialogueParser))]
public class NPC : Interactable
{
    public string npcName;
    public Sprite npcPortrait;
    public bool AssignedQuest = false;

    [Tooltip("Graph used for interactions with the NPC. Place any quests you want to give out in the graph itself.")]
    public DialogueGraph dialogueGraph;
    [fsIgnore]
    private DialogueGraph defaultDialogueGraph;
    public Quest activeQuest;
    [HideInInspector]
    public string dialogueGraphPath;

    private NPCManager npcManager;
    private GameEvents gameEvents;

    private void Awake()
    {
        npcManager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();
        npcManager.allNPCs.Add(this);
    }

    private void Start()
    {    
        gameEvents = GameObject.FindGameObjectWithTag("GameEvents").GetComponent<GameEvents>();

        dialogueGraph = Resources.Load<DialogueGraph>(dialogueGraphPath);
        GetComponent<DialogueParser>().graph = dialogueGraph;

        if(npcPortrait == null)
        {
            npcPortrait = PrefabManager.instance.unknownPersonPortrait;
        }

        try
        {
            ProgressSave progressSave = GameManager.instance.progressSave;            

            if (progressSave.npcStates.npcList.Any(x => x.npcName == npcName))
            {
                if (GameManager.instance.progressSave != null)
                {
                    NPC thisNPCSave = progressSave.npcStates.npcList.First(x => x.npcName == npcName);
                    AssignedQuest = thisNPCSave.AssignedQuest;
                    activeQuest = thisNPCSave.activeQuest;
                    dialogueGraphPath = thisNPCSave.dialogueGraphPath;
                }

                GetComponent<DialogueParser>().StopAllCoroutines();

                dialogueGraph = Resources.Load<DialogueGraph>(dialogueGraphPath);

                GetComponent<DialogueParser>().graph = dialogueGraph;
                GetComponent<DialogueParser>().isPlaying = false;
            }
        }
        catch
        {
            Debug.Log("No Game manager");
        }
    }    

    public override void Interact()
    {
        if(dialogueGraph != null)
        {
            base.Interact();
            gameEvents.OnNPCCommunicate(this);
            if (!AssignedQuest)
            {
                Debug.Log("Starting default graph");
                GetComponent<DialogueParser>().StartDialogueGraph();
            }
            else
            {
                Debug.Log("Starting quest graph");
                CheckQuestCompletion(activeQuest);
            }
        }
    }

    //Checks completion, if it is completed, completed dialogue plays and reward is given.
    //If it is not complete, in progress dialogue is played
    void CheckQuestCompletion(Quest questToCheck)
    {
        if (questToCheck.Completed)
        {
            questToCheck.GiveReward(gameObject);
            AssignedQuest = false;
            GetComponent<DialogueParser>().graph = questToCheck.questCompletedGraph;
            activeQuest = null;
            GetComponent<DialogueParser>().StartDialogueGraph();
        }
        else
        {
            GetComponent<DialogueParser>().graph = questToCheck.questInProgressGraph;
            GetComponent<DialogueParser>().StartDialogueGraph();
        }
    }
}
