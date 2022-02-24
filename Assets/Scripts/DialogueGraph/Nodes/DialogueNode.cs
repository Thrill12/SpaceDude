using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#718093")]
public class DialogueNode : BaseNode {

	[Input, SerializeField] public int entry;
	[Output, SerializeField] public int exit;

	public string dialogueText;

	public override string GetString()
    {
		return "Dialogue/" + dialogueText;
    }
}