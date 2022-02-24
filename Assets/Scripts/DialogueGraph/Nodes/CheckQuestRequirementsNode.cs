using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

[NodeTint("#44bd32")]
public class CheckQuestRequirementsNode : BaseNode
{
    [Input, SerializeField] public int entry;
    public Quest questToCheck;
    [Output, SerializeField] public int pass;
    [Output, SerializeField] public int fail;
    
    public override string GetString()
    {
        return "QuestCheckRequirements";
    }

    public Quest GetQuest()
    {
        return questToCheck;
    }
}