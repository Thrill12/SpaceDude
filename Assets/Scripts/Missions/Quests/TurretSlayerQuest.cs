using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSlayerQuest : Quest
{
    public override void Start()
    {
        base.Start();
        QuestName = "Turret slayer";
        QuestDescription = "You gotta slay them turrets man";

        //Could use this function to get data from a save file
        Goals.Add(new KillGoal(this, 1, "Killing turrets is a very good passtime", false, 0, 5));

        Goals.ForEach(goal => goal.Init());
    }
}
