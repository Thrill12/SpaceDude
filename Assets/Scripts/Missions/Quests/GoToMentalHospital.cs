using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMentalHospital : Quest
{
    public override void Start()
    {
        base.Start();
        QuestName = "Bringing inner peace to timmy";
        QuestDescription = "You were tasked by the mighty timmy to bring peace to his inner brain and going to the mental institute";

        Goals.Add(new LocationEnteredGoal(this, QuestName, QuestDescription, "Timmy's Mental Institute"));

        Goals.ForEach(goal => goal.Init());
    }
}
