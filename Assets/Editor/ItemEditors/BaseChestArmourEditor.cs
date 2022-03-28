using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseChestArmour))]
public class BaseChestArmourEditor : ItemEditor
{
    public SerializedProperty spriteNoWeaponsEquipped;
    [HideInInspector]
    public SerializedProperty spriteNoWeaponsEquippedPath;

    public SerializedProperty spriteSmallWeaponEquipped;
    [HideInInspector]
    public SerializedProperty spriteSmallWeaponEquippedPath;

    public SerializedProperty spriteLargeWeaponEquipped;
    [HideInInspector]
    public SerializedProperty spriteLargeWeaponEquippedPath;

    public override void OnEnable()
    {
        base.OnEnable();
        spriteNoWeaponsEquippedPath = serializedObject.FindProperty("spriteNoWeaponsEquippedPath");
        spriteNoWeaponsEquipped = serializedObject.FindProperty("spriteNoWeaponsEquipped");

        spriteSmallWeaponEquippedPath = serializedObject.FindProperty("spriteSmallWeaponEquippedPath");
        spriteSmallWeaponEquipped = serializedObject.FindProperty("spriteSmallWeaponEquipped");

        spriteLargeWeaponEquippedPath = serializedObject.FindProperty("spriteLargeWeaponEquippedPath");
        spriteLargeWeaponEquipped = serializedObject.FindProperty("spriteLargeWeaponEquipped");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (spriteNoWeaponsEquipped.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(spriteNoWeaponsEquipped.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            spriteNoWeaponsEquippedPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        if (spriteSmallWeaponEquipped.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(spriteSmallWeaponEquipped.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            spriteSmallWeaponEquippedPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        if (spriteLargeWeaponEquipped.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(spriteLargeWeaponEquipped.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            spriteLargeWeaponEquippedPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
