using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

[NodeTint("#8c7ae6")]
public class QuestNode : BaseNode {

	[Input] public int entry;
	[Output] public string exit;

	public Quest questToGive;

	public override string GetString()
    {
		return "Quest";
    }

	public Quest GetQuest()
    {
		return questToGive;
    }
}