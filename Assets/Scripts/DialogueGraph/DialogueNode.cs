using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : BaseNode {

	[Input] public int entry;
	[Output] public int exit;

	public string dialogueText;

	public override string GetString()
    {
		return "Dialogue/" + dialogueText;
    }
}