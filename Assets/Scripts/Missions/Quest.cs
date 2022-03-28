using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using FullSerializer;

[CreateAssetMenu(menuName ="Quests/Quest"), System.Serializable]
public class Quest : ScriptableObject
{
    public int id;
    public string QuestName;
    public string QuestDescription;
    public string npcAssignedTo;

    public DialogueGraph questCompletedGraph;
    [SerializeField, HideInInspector]
    public string questCompletedGraphPath;

    public DialogueGraph questInProgressGraph;
    [SerializeField, HideInInspector]
    public string questInProgressGraphPath;

    public List<Goal> Goals = new List<Goal>();

    [Space(10)]

    public int ExperienceReward;
    public int FactionInfluenceReward;
    public BaseItem ItemReward;
    public bool Completed;

    [Space(10)]

    [Space(10)]
    public List<Quest> questRequirements;

    [HideInInspector]
    public bool HandedIn;

    //Spawns a clone of the goals, so that we don't modify the ones in the assets folder
    public void Init()
    {
        GameManager.instance.LoadResourcesForQuest(this);

        for (int i = 0; i < Goals.Count; i++)
        {
            Goals[i] = Instantiate(Goals[i]);
        }
        GameManager.instance.uiManager.DrawQuest(this);
        GameManager.instance.uiManager.audioSource.PlayOneShot(PrefabManager.instance.questAssignedSound);
    }

    public bool CheckGoals()
    {
        //Returns true only if all the goals in the goals list are completed
        Completed = Goals.All(x => x.GoalCompleted == true);

        if (Completed)
        {
            Debug.Log("Completed quest '" + QuestName + "' - go back to quest giver for reward");
        }

        return Completed;
    }

    //Gives rewards for the quest
    public void GiveReward(GameObject position)
    {
        GameManager.instance.questManager.RemoveQuest(this);
        GameManager.instance.uiManager.RemoveQuest(this);

        if(ItemReward != null)
        {
            PrefabManager.instance.SpawnItem(position, ItemReward);
        }

        //implement xp and faction XP

        HandedIn = true;
    }

    //Bool to check if the quest can be assigned to the player or not based on if all its requirement quests have been completed
    public bool CheckRequirements()
    {
        Debug.Log("Checking for " + QuestName);

        if(questRequirements.Count > 0)
        {
            //returns true only if all reqs are completed and their own reqs are completed and pass reqs
            foreach (var item in questRequirements)
            {
                if(GameManager.instance.questManager.completedQuests.Count > 0)
                {
                    if (GameManager.instance.questManager.completedQuests.First(x => x.id == item.id && x.Completed && x.CheckRequirements() && x.HandedIn))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        else
        {
            return true;
            //Quest has no requirements
        }
    }
}
