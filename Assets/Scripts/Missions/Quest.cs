using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string QuestDescription { get; set; }
    public int ExperienceReward { get; set; }
    public int FactionInfluenceReward { get; set; }
    public BaseItem ItemReward { get; set; }
    public bool Completed { get; set; }

    public List<Quest> questRequirements;

    private GameObject player;

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerSuit");
    }

    public void CheckGoals()
    {
        //Returns true only if all the goals in the goals list are completed
        Completed = Goals.All(x => x.GoalCompleted == true);

        if (Completed)
        {
            Debug.Log("Completed quest " + QuestName + " - go back to quest giver for reward");           
        }
    }

    //Gives rewards for the quest
    public void GiveReward()
    {
        Debug.Log("Given reward to player");
        if(ItemReward != null)
        {
            PrefabManager.instance.SpawnItem(player, ItemReward);
        }

        //implement xp and faction XP
    }
}
