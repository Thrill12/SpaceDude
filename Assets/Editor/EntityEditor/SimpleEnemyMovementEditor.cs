using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleEnemyMovement))]
public class SimpleEnemyMovementEditor : Editor
{
    private void OnSceneGUI()
    {
        SimpleEnemyMovement movement = target as SimpleEnemyMovement;

        Handles.color = Color.white;
        Handles.DrawWireArc(movement.transform.position, Vector3.forward, Vector3.right, 360, movement.visionRadius);
        Vector3 viewAngleA = movement.DirectionFromAngle(-movement.visionAngle / 2, false);
        Vector3 viewAngleB = movement.DirectionFromAngle(movement.visionAngle / 2, false);
        Handles.color = Color.green;
        Handles.DrawLine(movement.transform.position, movement.transform.position + viewAngleA * movement.visionRadius);
        Handles.DrawLine(movement.transform.position, movement.transform.position + viewAngleB * movement.visionRadius);
    }
}
