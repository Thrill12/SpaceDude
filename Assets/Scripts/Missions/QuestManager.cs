using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> allQuests = new List<Quest>();

    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    private void Awake()
    {
        instance = this;
    }

    //Each quest giver will run this function at start up, having all the quests in one place will be useful later down the line
    public void AddQuestToOverallPool(Quest quest)
    {
        allQuests.Add(quest);
    }

    //Ads a quest to the active quests list
    public void AddQuest(Quest questToAdd)
    {
        activeQuests.Add(questToAdd);

        questToAdd.Init();

        foreach (var item in questToAdd.Goals)
        {
            item.Init(questToAdd);
        }

        if(questToAdd.Goals.Count == 0)
        {
            Debug.Log("Uhm good sir, there seems to be no goal on the " + questToAdd.QuestName + " quest.");
        }
    }

    //Removes a quest from active and places it in completed quests
    public void RemoveQuest(Quest questToRemove)
    {
        activeQuests.Remove(questToRemove);
        completedQuests.Add(questToRemove);
    }
}
