using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SetNPCToGraphNode : BaseNode {

	[Input, SerializeField] public int entry;

	public string npcToChange;

    public DialogueGraph graphToChangeTo;
    [SerializeField, HideInInspector]
    public string graphToChangeToPath;

	[Output, SerializeField] public int exit;

    public override string GetString()
    {
        return "SetNPCToGraph/" + graphToChangeToPath;
    }
}