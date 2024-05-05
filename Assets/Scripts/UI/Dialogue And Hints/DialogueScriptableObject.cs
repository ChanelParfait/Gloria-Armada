using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Script")]
public class DialogueScriptableObject : ScriptableObject {
    public List<Speaker> speakers; // This will be populated with Speaker ScriptableObjects
    public List<DialogueLine> lines;
    public float timeBetweenLines = 1.0f;
}