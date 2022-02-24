using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseWeapon), true)]
public class BaseWeaponEditor : ItemEditor
{
    public SerializedProperty weaponObject;
    [HideInInspector]
    public SerializedProperty weaponObjectPath;

    public SerializedProperty attackAudioClip;
    [HideInInspector]
    public SerializedProperty attackAudioClipPath;

    public override void OnEnable()
    {
        base.OnEnable();

        weaponObject = serializedObject.FindProperty("weaponObject");
        weaponObjectPath = serializedObject.FindProperty("weaponObjectPath");

        attackAudioClip = serializedObject.FindProperty("attackSound");
        attackAudioClipPath = serializedObject.FindProperty("attackSoundPath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (weaponObject.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(weaponObject.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            weaponObjectPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        if (attackAudioClip.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(attackAudioClip.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            attackAudioClipPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        base.OnInspectorGUI();        
    }
}
