using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Goals/Kill Goal")]
public class KillGoal : Goal
{
    public int EnemyId;

    public KillGoal(Quest quest, int EnemyId, string description, bool completed, int currentAmount, int requiredAmount)
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
