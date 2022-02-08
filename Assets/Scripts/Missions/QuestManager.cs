using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Start()
    {
        LoadQuests();

        foreach (var quest in activeQuests)
        {
            quest.Init();

            foreach (var goal in quest.Goals)
            {
                goal.Init(quest);
            }
        }
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
        SaveQuests();

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

    public void AddQuest(Quest questToAdd, NPC questGiver)
    {       
        Quest newQuest = Instantiate(questToAdd);
        questGiver.activeQuest = newQuest;
        activeQuests.Add(newQuest);
        questGiver.AssignedQuest = true;

        newQuest.Init();

        foreach (var item in newQuest.Goals)
        {
            item.Init(newQuest);
        }

        if (newQuest.Goals.Count == 0)
        {
            Debug.Log("Uhm good sir, there seems to be no goal on the " + newQuest.QuestName + " quest.");
        }
    }

    //Removes a quest from active and places it in completed quests
    public void RemoveQuest(Quest questToRemove)
    {
        activeQuests.Remove(questToRemove);
        completedQuests.Add(questToRemove);
        SaveQuests();
    }

    public bool IsCompleted(Quest questToCheck)
    {
        if(completedQuests.Any(x => x.id == questToCheck.id && x.HandedIn))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCompleted(int questId)
    {
        Debug.Log("Checking quest with id " + questId);

        foreach (var item in completedQuests)
        {
            Debug.Log("Quest id is found at " + item.id);
        }

        if(completedQuests.Count > 0)
        {
            Debug.Log("Quests with id " + questId + " are " + completedQuests.First(x => x.id == questId));
        }
        
        if (completedQuests.Any(x => x.id == questId && x.HandedIn))
        { 
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SaveQuests()
    {
        GameManager.instance.progressSave.questsSaved = new QuestsSavedSO();
        foreach (var item in activeQuests)
        {
            GameManager.instance.progressSave.questsSaved.questsActive.Add(item);
        }
        foreach (var item in completedQuests)
        {
            GameManager.instance.progressSave.questsSaved.questsCompleted.Add(item);
        }

        GameManager.instance.SaveProgress();
        Debug.Log("Saved quests");
    }

    public void LoadQuests()
    {
        foreach (var item in GameManager.instance.progressSave.questsSaved.questsActive)
        {
            activeQuests.Add((Quest)item);
        }

        foreach (var item in GameManager.instance.progressSave.questsSaved.questsCompleted)
        {
            completedQuests.Add((Quest)item);
        }
    }
}
