using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseItem), true)]
public class ItemEditor : Editor
{
    public SerializedProperty itemIcon;
    [HideInInspector]
    public SerializedProperty itemIconPath;  

    public const string resourcesFolderPrefix = "Assets/Resources";

    public virtual void OnEnable()
    {
        itemIconPath = serializedObject.FindProperty("itemIconPath");
        itemIcon = serializedObject.FindProperty("itemIcon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (itemIcon.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(itemIcon.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            itemIconPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();        
    }
}
