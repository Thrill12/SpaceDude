using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseGun), true)]
public class BaseGunEditor : BaseWeaponEditor
{
    public SerializedProperty projectileObject;
    [HideInInspector]
    public SerializedProperty projectileObjectPath;

    public SerializedProperty outOfAmmoAudioClip;
    [HideInInspector]
    public SerializedProperty outOfAmmoAudioClipPath;

    public override void OnEnable()
    {
        base.OnEnable();

        projectileObject = serializedObject.FindProperty("projectile");
        projectileObjectPath = serializedObject.FindProperty("projectilePath");

        outOfAmmoAudioClip = serializedObject.FindProperty("outOfAmmoSound");
        outOfAmmoAudioClipPath = serializedObject.FindProperty("outOfAmmoSoundFilePath");
    }

    public override void OnInspectorGUI()
    {
        if (projectileObject.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(projectileObject.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            projectileObjectPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        if (outOfAmmoAudioClip.objectReferenceValue != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(outOfAmmoAudioClip.objectReferenceValue.GetInstanceID());
            if (assetPath.StartsWith(resourcesFolderPrefix))
            {
                assetPath = assetPath.Substring(resourcesFolderPrefix.Length + 1);
                assetPath = assetPath.Split(".")[0];
            }
            outOfAmmoAudioClipPath.stringValue = assetPath;
            serializedObject.ApplyModifiedProperties();
        }

        base.OnInspectorGUI();        
    }
}
