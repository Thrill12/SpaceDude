using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGoal : Goal
{
    public int EnemyId { get; set; }

    public KillGoal(Quest quest, int EnemyId, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.goalQuest = quest;
        this.EnemyId = EnemyId;
        this.GoalDescription = description;
        this.GoalCompleted = completed;
        this.GoalCurrentAmount = currentAmount;
        this.GoalRequiredAmount = requiredAmount;
    }

    public override void Init()
    {
        base.Init();
        GameEvents.instance.onEntityKilled += EnemyDied;
    }

    void EnemyDied(BaseEntity enemy)
    {
        if(enemy.ID == EnemyId)
        {
            GoalCurrentAmount++;
            Evaluate();
            Debug.Log("Progress made hehe");
        }
    }
}
