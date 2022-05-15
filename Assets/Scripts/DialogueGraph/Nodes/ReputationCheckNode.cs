using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#44bd32")]
public class ReputationCheckNode : BaseNode
{
    [Input, SerializeField] public int entry;
    public string factionName;
    public int factionRequiredLevel;
    [Output, SerializeField] public int pass;
    [Output, SerializeField] public int fail;

    public override string GetString()
    {
        return "FactionCheckReputation/" + factionName + "/" + factionRequiredLevel;
    }
}
