using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CheckQuestCompletionNode : BaseNode {

	[Input, SerializeField] public int entry;
	public Quest questToCheck;
	[Output, SerializeField] public int completed;
	[Output, SerializeField] public int notcompleted;

	public override string GetString()
    {
		return "QuestCheckCompletion/" + questToCheck.id.ToString();
    }
}