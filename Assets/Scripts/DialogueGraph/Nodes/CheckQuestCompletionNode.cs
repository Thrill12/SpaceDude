using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CheckQuestCompletionNode : BaseNode {

	[Input] public int entry;
	public Quest questToCheck;
	[Output] public int completed;
	[Output] public int notcompleted;

	public override string GetString()
    {
		return "QuestCheckCompletion/" + questToCheck.id.ToString();
    }
}