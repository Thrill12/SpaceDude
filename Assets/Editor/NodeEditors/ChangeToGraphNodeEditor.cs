using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChangeToGraphNode))]
public class ChangeToGraphNodeEditor : Editor
{
    public SerializedProperty graphToChangeTo;
    [HideInInspector]
    public SerializedProperty graphToChangeToPath;

    public const string resourcesFolderPrefix = "Assets/Resources";

    public virtual void OnEnable()
    {
        graphToChangeTo = serializedObject.FindProperty("graphToChangeTo");
        graphToChangeToPath = serializedObject.FindProperty("graphToChangeToPath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (graphToChangeTo.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(graphToChangeTo.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            graphToChangeToPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
