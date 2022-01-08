using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    public Quest goalQuest { get; set; }
    public string GoalName { get; set; }
    public string GoalDescription { get; set; }
    public bool GoalCompleted { get; set; }
    public int GoalCurrentAmount { get; set; }
    public int GoalRequiredAmount { get; set; }

    public virtual void Init()
    {
        // default init
    }

    public void Evaluate()
    {
        if(GoalCurrentAmount >= GoalRequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        GoalCompleted = true;
        goalQuest.CheckGoals();
    }
}
