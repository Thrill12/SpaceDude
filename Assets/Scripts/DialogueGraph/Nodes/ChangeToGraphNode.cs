using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ChangeToGraphNode : BaseNode {

	[Input, SerializeField] public int entry;

    public DialogueGraph graphToChangeTo;
    [SerializeField, HideInInspector]
    public string graphToChangeToPath;


    public bool startImmediately = false;
	[Output, SerializeField] public int exit;

	public override string GetString()
    {
		return "GraphChange/" + startImmediately.ToString() + "/" + graphToChangeToPath;
    }
}