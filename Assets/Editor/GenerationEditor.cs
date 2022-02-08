using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratorEditor))]
public class GeneratorEditor : Editor
{
    //Overrides the drawing of the inspector for the room component.
    public override void OnInspectorGUI()
    {
        //Cast the target  - the room component which we want the editor for - to 'Room'.
        GenDebugger gen = (GenDebugger)target;

        //Create a button with the label "Generate Room Data".
        if (GUILayout.Button("Generate"))
        {
            gen.Start();
        }

        //Displays the varaibles visible to the inspector underneath the button.
        base.OnInspectorGUI();
    }

}
