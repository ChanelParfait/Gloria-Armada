using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (DialogueWriter))]
public class DialogueWriter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // serializedObject.Update();  // Always call this at the start of OnInspectorGUI.

        // ActionHandler handler = (ActionHandler)target;

        // // Always draw the enum popup
        // EditorGUILayout.PropertyField(serializedObject.FindProperty("actionType"), new GUIContent("Action Type"));

        // // Conditional fields based on the enum value
        // switch (handler.actionType)
        // {
        //     case ActionType.Move:
        //         EditorGUILayout.PropertyField(serializedObject.FindProperty("moveParameters"), new GUIContent("Move Parameters"));
        //         break;
        //     case ActionType.Rotate:
        //         EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationAngle"), new GUIContent("Rotation Angle"));
        //         break;
        //     case ActionType.Scale:
        //         EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleParameters"), new GUIContent("Scale Parameters"));
        //         break;
        // }

        // serializedObject.ApplyModifiedProperties();  // Apply changes to all serializedProperties
    }
}
