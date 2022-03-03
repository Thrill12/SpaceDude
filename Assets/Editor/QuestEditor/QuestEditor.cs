using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    SerializedProperty questCompletedGraph;
    SerializedProperty questCompletedGraphPath;

    SerializedProperty questInProgressGraph;
    SerializedProperty questInProgressGraphPath;

    public const string resourcesFolderPrefix = "Assets/Resources";

    public virtual void OnEnable()
    {
        questCompletedGraphPath = serializedObject.FindProperty("questCompletedGraphPath");
        questCompletedGraph = serializedObject.FindProperty("questCompletedGraph");

        questInProgressGraphPath = serializedObject.FindProperty("questInProgressGraphPath");
        questInProgressGraph = serializedObject.FindProperty("questInProgressGraph");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (questCompletedGraph.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(questCompletedGraph.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            questCompletedGraphPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        if (questInProgressGraph.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(questInProgressGraph.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            questInProgressGraphPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
