using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Goal : ScriptableObject
{
    [SerializeReference] public Quest goalQuest;
    public string GoalName;
    public string GoalDescription;
    [Header("This is what will display in the quest log - if you set a required amount it will do current/req + below")]
    [Header("If you do not put a required amount, it just places the below")]
    public string GoalQuestLogDisplay;
    public bool GoalCompleted;
    public int GoalCurrentAmount;
    public int GoalRequiredAmount;

    public virtual void Init(Quest quest)
    {
        // default init
        goalQuest = quest;
    }

    public void Evaluate()
    {
        QuestManager.instance.SaveQuests();
        if(GoalCurrentAmount >= GoalRequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        if (!GoalCompleted)
        {
            UIManager.instance.audioSource.PlayOneShot(PrefabManager.instance.questGoalCompletedSound);
            GoalCompleted = true;
            goalQuest.CheckGoals();
            QuestManager.instance.SaveQuests();
        }       
    }
}