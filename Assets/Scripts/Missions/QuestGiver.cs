using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : NPC
{
    public bool AssignedQuests { get; set; } = false;
    public bool Helped { get; set; } = false;

    [SerializeField]
    private GameObject quests;

    [SerializeField]
    private string questType;

    private Quest Quest { get; set; }

    public override void Interact()
    {
        base.Interact();
        if(!AssignedQuests && !Helped)
        {
            AssignQuest();
            UIManager.instance.DisplayDialogue(new List<string> { "Howdy partner! Thanks for lending me a hand.", "Ill be sure to reward you with a nice debug log." }, npcName);
        }
        else if (AssignedQuests && !Helped)
        {
            CheckQuestCompletion();
        }
        else
        {
            UIManager.instance.DisplayDialogue(new List<string> { "Bruv, you've already finished the quest and Andrei hasn't done it so i can give you multiple quests.", "Come back after hes done that!" }, npcName);
        }
    }

    void AssignQuest()
    {
        AssignedQuests = true;
        Quest = (Quest)quests.AddComponent(System.Type.GetType(questType));
    }

    void CheckQuestCompletion()
    {
        if (Quest.Completed)
        {
            Quest.GiveReward();
            Helped = true;
            AssignedQuests = false;
            UIManager.instance.DisplayDialogue("YEAHHHH U DID THE THING AND NOW YOU GOT THE THING FROM ME THANKS BABES", npcName);
        }
        else
        {
            UIManager.instance.DisplayDialogue("Oi oi, u haven't completed me quest m8. Get gooder", npcName);
        }
    }
}
