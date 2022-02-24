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
    public bool AssignedQuest = false;

    [Tooltip("Graph used for interactions with the NPC. Place any quests you want to give out in the graph itself.")]
    public DialogueGraph dialogueGraph;
    [fsIgnore]
    private DialogueGraph defaultDialogueGraph;
    public Quest activeQuest;

    public string dialogueGraphPath;

    private void Start()
    {
        defaultDialogueGraph = dialogueGraph;        

        ProgressSave progressSave = GameManager.instance.progressSave;

        if (progressSave.npcStates.npcList.Any(x => x.npcName == npcName))
        {
            if (GameManager.instance.progressSave != null)
            {
                NPC thisNPCSave = progressSave.npcStates.npcList.First(x => x.npcName == npcName);
                AssignedQuest = thisNPCSave.AssignedQuest;
                dialogueGraph = thisNPCSave.dialogueGraph;
                activeQuest = thisNPCSave.activeQuest;
            }

            GetComponent<DialogueParser>().graph = dialogueGraph;
            GetComponent<DialogueParser>().isPlaying = false;
        }
        else
        {
            NPCManager.instance.allNPCs.Add(this);
        }
    }    

    private void Update()
    {
        if(activeQuest == null && GetComponent<DialogueParser>().isPlaying == false)
        {
            dialogueGraph = defaultDialogueGraph;
            GetComponent<DialogueParser>().graph = dialogueGraph;
        }
    }

    public override void Interact()
    {
        base.Interact();
        GameEvents.instance.OnNPCCommunicate(this);
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

    //Checks completion, if it is completed, completed dialogue plays and reward is given.
    //If it is not complete, in progress dialogue is played
    void CheckQuestCompletion(Quest questToCheck)
    {
        if (questToCheck.Completed)
        {
            questToCheck.GiveReward(gameObject);
            AssignedQuest = false;
            GetComponent<DialogueParser>().graph = Resources.Load(GameManager.instance.questsPath + "/" + activeQuest.questCompletedDialoguePath) as DialogueGraph;
            activeQuest = null;
            GetComponent<DialogueParser>().StartDialogueGraph();
        }
        else
        {
            GetComponent<DialogueParser>().graph = Resources.Load(GameManager.instance.questsPath + "/" + activeQuest.questInProgressDialoguePath) as DialogueGraph;
            GetComponent<DialogueParser>().StartDialogueGraph();
        }
    }
}
