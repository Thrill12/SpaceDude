using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestGiver : NPC
{
    public bool AssignedQuest { get; set; } = false;

    public List<Quest> availableQuests;

    public Quest activeQuest;

    private void Start()
    {
        for (int i = 0; i < availableQuests.Count; i++)
        {
            availableQuests[i] = ScriptableObject.Instantiate(availableQuests[i]);
            QuestManager.instance.AddQuestToOverallPool(availableQuests[i]);
        }
    }

    public override void Interact()
    {
        base.Interact();
        if(!AssignedQuest)
        {
            AssignQuest();
        }
        else
        {
            CheckQuestCompletion(activeQuest);
        }
    }

    void AssignQuest()
    {     
        List<Quest> possibleQuests = new List<Quest>();

        foreach (var item in availableQuests)
        {
            if(item.CheckRequirements() == true)
            {
                Debug.Log("Came out true with " + item);
                possibleQuests.Add(item);
            }
        }

        if(possibleQuests.Count > 0)
        {    
            AssignedQuest = true;
            activeQuest = possibleQuests[0];
            availableQuests.Remove(activeQuest);
            Debug.Log(activeQuest.QuestName);
            QuestManager.instance.AddQuest(activeQuest);
            UIManager.instance.DisplayDialogue(activeQuest.questAssignedDialogue, npcName);
        }
        else
        {
            UIManager.instance.DisplayDialogue("Hey uh, sorry m8 I haven't any quests for you today. Maybe come back later?", npcName);
        }
    }

    void CheckQuestCompletion(Quest questToCheck)
    {
        if (questToCheck.Completed)
        {
            questToCheck.GiveReward(gameObject);
            AssignedQuest = false;
            UIManager.instance.DisplayDialogue(activeQuest.questCompletedDialogue, npcName);
            activeQuest = null;
        }
        else
        {
            UIManager.instance.DisplayDialogue(activeQuest.questInProgressDialogue, npcName);
        }
    }
}
