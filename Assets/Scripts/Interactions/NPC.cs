using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueParser))]
public class NPC : Interactable
{
    public string npcName;

    public bool AssignedQuest { get; set; } = false;

    [Tooltip("Graph used for interactions with the NPC. Place any quests you want to give out in the graph itself - got rid of the weird old way of doing it")]
    public DialogueGraph dialogueGraph;
    public Quest activeQuest;

    private void Start()
    {
        GetComponent<DialogueParser>().graph = dialogueGraph;
    }

    public override void Interact()
    {
        base.Interact();
        if (!AssignedQuest)
        {
            
            GetComponent<DialogueParser>().StartDialogueGraph();
        }
        else
        {
            CheckQuestCompletion(activeQuest);
            GetComponent<DialogueParser>().StartDialogueGraph();
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
            GetComponent<DialogueParser>().graph = activeQuest.questCompletedDialogue;
            activeQuest = null;
            Debug.Log("Finished");
        }
        else
        {
            Debug.Log("Not Finishjed");
            GetComponent<DialogueParser>().graph = activeQuest.questInProgressDialogue;
        }
    }
}
