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

    //If there is no assigned quest from this npc, assign a quest, otherwise check for completion of current quest
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

    //Will check through available quests from this npc, and will return any possible quests.
    //If returns multiple, it returns the first one, so we could have quests in order
    void AssignQuest()
    {     
        //Pool of available quests to give to the player
        List<Quest> possibleQuests = new List<Quest>();

        foreach (var item in availableQuests)
        {
            //Checks if the quest's requirements are completed, and if so id adds the quest to the pool of quests
            if(item.CheckRequirements() == true)
            {
                possibleQuests.Add(item);
            }
        }

        //If there are any quests in the pool, it picks the first one. Likely, quests will be placed in the story order in the inspector,
        //so i chose to pick the first element of the list, but we can change that if need be
        if(possibleQuests.Count > 0)
        {    
            AssignedQuest = true;
            activeQuest = possibleQuests[0];
            availableQuests.Remove(activeQuest);
            QuestManager.instance.AddQuest(activeQuest);
            UIManager.instance.DisplayDialogue(activeQuest.questAssignedDialogue, npcName);
        }
        else
        {
            UIManager.instance.DisplayDialogue("Hey uh, sorry m8 I haven't any quests for you today. Maybe come back later?", npcName);
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
            UIManager.instance.DisplayDialogue(activeQuest.questCompletedDialogue, npcName);
            activeQuest = null;
        }
        else
        {
            UIManager.instance.DisplayDialogue(activeQuest.questInProgressDialogue, npcName);
        }
    }
}
