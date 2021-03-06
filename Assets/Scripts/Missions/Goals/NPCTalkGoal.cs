using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Goals/NPC Talk Goal"), System.Serializable]
public class NPCTalkGoal : Goal
{
    public string npcToTalkToName;

    public NPCTalkGoal(string npcToTalkToName)
    {
        this.npcToTalkToName = npcToTalkToName;
    }

    public override void Init(Quest quest)
    {
        base.Init(quest);
        GameManager.instance.gameEvents.onNPCCommunicate += OnTalkToNPC;
    }

    public void OnTalkToNPC(NPC npc)
    {
        if (npc.npcName == npcToTalkToName)
        {
            Complete();
        }
    }
}
