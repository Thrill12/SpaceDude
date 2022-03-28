using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPC))]
public class NPCEditor : Editor
{
    public SerializedProperty graph;
    [HideInInspector]
    public SerializedProperty graphPath;

    public const string resourcesFolderPrefix = "Assets/Resources";

    public virtual void OnEnable()
    {
        graph = serializedObject.FindProperty("dialogueGraph");
        graphPath = serializedObject.FindProperty("dialogueGraphPath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (graph.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(graph.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            graphPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
