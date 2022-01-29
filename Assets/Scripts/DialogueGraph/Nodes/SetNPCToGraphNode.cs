using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SetNPCToGraphNode : BaseNode {

	[Input] public int entry;

	public string npcToChange;
	public DialogueGraph graphToChangeTo;

	[Output] public int exit;

    public override string GetString()
    {
        return "SetNPCToGraph";
    }
}