using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueScriptableObject))]
public class DialogueScriptableObjectEditor : Editor
{
    private Color GetFactionColor(Faction faction)
    {
        switch (faction)
        {
            case Faction.Player:
                return Color.blue;
            case Faction.Squad:
                return Color.green;
            case Faction.Friendly:
                return Color.cyan;
            case Faction.Enemy:
                return Color.red;
            case Faction.Neutral:
                return Color.gray;
            default:
                return GUI.backgroundColor; // Default background color
        }
    }

    public override void OnInspectorGUI()
    {
        DialogueScriptableObject scriptableObj = (DialogueScriptableObject)target;

        // Allow manual modification of the serialized properties
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("speakers"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenLines"));

        // Handling Dialogue Lines
        SerializedProperty lines = serializedObject.FindProperty("lines");
        EditorGUILayout.PropertyField(lines, new GUIContent("Dialogue Lines"), false);

        if (lines.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < lines.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty line = lines.GetArrayElementAtIndex(i);
                SerializedProperty speaker = line.FindPropertyRelative("speaker");
                SerializedProperty lineText = line.FindPropertyRelative("line");
                Speaker speakerObj = (Speaker)speaker.objectReferenceValue;

                Color defaultColor = GUI.backgroundColor; // Save the default background color
                if (speakerObj != null)
                {
                    GUI.contentColor = GetFactionColor(speakerObj.faction); // Change background based on faction
                }

                string lineLabel = string.IsNullOrEmpty(lineText.stringValue) ? $"New Dialogue Line {i}" : lineText.stringValue;
                EditorGUILayout.PropertyField(line, new GUIContent(lineLabel), false);
                
                GUI.contentColor = defaultColor; // Reset to the default color

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    lines.DeleteArrayElementAtIndex(i);
                    lines.serializedObject.ApplyModifiedProperties(); // Apply changes immediately
                    EditorGUILayout.EndHorizontal();
                    break; // Exit the loop to prevent accessing invalid dat
                }
                EditorGUILayout.EndHorizontal();

                if (line.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(speaker, new GUIContent("Speaker"));
                    EditorGUILayout.PropertyField(line.FindPropertyRelative("dialogueType"), new GUIContent("Dialogue Type"));
                    EditorGUILayout.PropertyField(line.FindPropertyRelative("line"), new GUIContent("Line"));

                    DialogueType dialogueType = (DialogueType)line.FindPropertyRelative("dialogueType").enumValueIndex;
                    if (dialogueType == DialogueType.Choice)
                    {
                        EditorGUILayout.PropertyField(line.FindPropertyRelative("choice"), new GUIContent("Choice"), true);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            if (GUILayout.Button("Add Dialogue Line"))
            {
                lines.InsertArrayElementAtIndex(lines.arraySize);
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
