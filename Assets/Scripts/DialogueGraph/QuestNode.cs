using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

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