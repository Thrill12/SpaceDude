using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseConsumable), true)]
public class BaseConsumableEditor : ItemEditor
{
    public SerializedProperty useSound;
    [HideInInspector]
    public SerializedProperty useSoundPath;

    public override void OnEnable()
    {
        base.OnEnable();

        useSound = serializedObject.FindProperty("useSound");
        useSoundPath = serializedObject.FindProperty("useSoundPath");
    }

    public override void OnInspectorGUI()
    {
        if (useSound.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(useSound.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            useSoundPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        base.OnInspectorGUI();
    }
}
