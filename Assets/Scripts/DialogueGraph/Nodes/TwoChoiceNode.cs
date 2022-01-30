using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XNode;

[NodeTint("#c23616")]
public class TwoChoiceNode : BaseNode
{
    [Input] public int entry;
    
    public string dialogue;

    public string passButtonString;
    [Output] public int pass;
    public string failButtonString;
	[Output] public int fail;

	public override string GetString()
    {
		return "TwoChoice/" + dialogue + "/" + passButtonString + "/" + failButtonString;
    }
}