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
        InitializeQuests();
    }

    public void InitializeQuests()
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

    public void AddQuest(Quest questToAdd, NPC questGiver)
    {       
        Quest newQuest = Instantiate(questToAdd);
        newQuest.npcAssignedTo = questGiver.npcName;
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

        SaveQuests();
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
        try
        {
            QuestsSavedSO save = new QuestsSavedSO();

            foreach (var item in activeQuests)
            {
                save.questsActive.Add(item);
            }
            foreach (var item in completedQuests)
            {
                save.questsCompleted.Add(item);
            }

            ProgressSave progressSavee = GameManager.instance.progressSave as ProgressSave;
            progressSavee.questsSaved = save;
            Debug.Log("Saved quests");
        }
        catch
        {
            Debug.LogWarning("No Game Manager");
        }      
    }

    public void LoadQuests()
    {
        var save = GameManager.instance.progressSave as ProgressSave;

        if (save == null) return;

        if (save.questsSaved == null) return;

        QuestsSavedSO savee = save.questsSaved as QuestsSavedSO;
        List<NPC> npcSaves = save.npcStates.npcList.Cast<NPC>().ToList();

        foreach (var item in savee.questsActive)
        {
            Quest questToAdd = (Quest)item;
            NPC questGiver = NPCManager.instance.allNPCs.First(x => x.npcName == questToAdd.npcAssignedTo);

            Quest newQuest = Instantiate(questToAdd);
            newQuest.npcAssignedTo = questGiver.npcName;
            questGiver.activeQuest = newQuest;
            questGiver.AssignedQuest = true;

            newQuest.Init();

            foreach (var goal in newQuest.Goals)
            {
                goal.Init(newQuest);
            }

            if (newQuest.Goals.Count == 0)
            {
                Debug.Log("Uhm good sir, there seems to be no goal on the " + newQuest.QuestName + " quest.");
            }            
        }

        foreach (var item in savee.questsCompleted)
        {
            Quest questToAdd = (Quest)item;
            completedQuests.Add(questToAdd);
        }
    }
}
