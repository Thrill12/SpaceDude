using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    //Overrides the drawing of the inspector for the room component.
    public override void OnInspectorGUI()
    {
        //Cast the target  - the room component which we want the editor for - to 'Room'.
        Room room = (Room)target;

        //Create a button with the label "Generate Room Data".
        if (GUILayout.Button("Generate Room Data"))
        {   //Runs when the button is pressed:
            Debug.Log("Generating Room Data.");
            //Get the room component to generate the room data. 
            RoomData data = room.GenerateRoomData();
            //Take the room data and save that to the rooms folder, if a room data obj already exists with the same name its overwritten.
            AssetDatabase.CreateAsset(data, "Assets/Data/Rooms/" + room.name + ".asset");
        }

        //Displays the varaibles visible to the inspector underneath the button.
        base.OnInspectorGUI();
    }

}
