using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XNode;

[NodeTint("#c23616")]
public class TwoChoiceNode : BaseNode
{
    [Input, SerializeField] public int entry;
    
    public string dialogue;

    public string passButtonString;
    [Output, SerializeField] public int pass;
    public string failButtonString;
	[Output, SerializeField] public int fail;

	public override string GetString()
    {
		return "TwoChoice/" + dialogue + "/" + passButtonString + "/" + failButtonString;
    }
}