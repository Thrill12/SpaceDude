using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ChangeToGraphNode : BaseNode {

	[Input] public int entry;
	public DialogueGraph graphToChangeTo;
	public bool startImmediately = false;
	[Output] public int exit;

	public override string GetString()
    {
		return "GraphChange/" + startImmediately.ToString();
    }

	public DialogueGraph GetGraph()
    {
		return graphToChangeTo;
    }
}