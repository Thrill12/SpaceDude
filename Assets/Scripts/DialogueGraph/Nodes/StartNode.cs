using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#0097e6")]
public class StartNode : BaseNode {

	[Output] public int exit;

	public override string GetString()
    {
        return "Start";
    }
}