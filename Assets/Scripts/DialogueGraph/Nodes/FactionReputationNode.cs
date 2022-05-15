using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#40739e")]
public class FactionReputationNode : BaseNode
{
    [Input, SerializeField] public int entry;
	public string factionName;
    public int reputationToAdd;
	[Output, SerializeField] public int exit;

	public override string GetString()
	{
		return "Reputation/" + factionName + "/" + reputationToAdd;
	}
}
