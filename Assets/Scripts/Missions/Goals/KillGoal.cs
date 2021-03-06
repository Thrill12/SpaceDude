using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Goals/Kill Goal"), System.Serializable]
public class KillGoal : Goal
{
    public string EnemyId;

    public KillGoal(Quest quest, string EnemyId, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.goalQuest = quest;
        this.EnemyId = EnemyId;
        this.GoalDescription = description;
        this.GoalCompleted = completed;
        this.GoalCurrentAmount = currentAmount;
        this.GoalRequiredAmount = requiredAmount;
    }

    public override void Init(Quest quest)
    {
        base.Init(quest);
        GameManager.instance.gameEvents.onEntityKilled += EnemyDied;
    }

    void EnemyDied(BaseEntity enemy, BaseEntity killer)
    {
        if(enemy.ID == EnemyId && killer.entityName == "Player")
        {
            GoalCurrentAmount++;
            Evaluate();
        }
    }
}
