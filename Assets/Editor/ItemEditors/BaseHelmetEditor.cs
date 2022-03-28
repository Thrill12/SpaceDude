using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseHelmet))]
public class BaseHelmetEditor : ItemEditor
{
    public SerializedProperty showableSprite;
    [HideInInspector]
    public SerializedProperty showableSpritePath;

    public override void OnEnable()
    {
        base.OnEnable();
        showableSpritePath = serializedObject.FindProperty("showableSpritePath");
        showableSprite = serializedObject.FindProperty("showableSprite");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (showableSprite.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(showableSprite.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            showableSpritePath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
