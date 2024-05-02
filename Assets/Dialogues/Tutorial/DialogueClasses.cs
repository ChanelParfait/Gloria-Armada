using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum SpeakerType
{
    Player,
    NPC
}

[System.Serializable]
public enum Faction
{
    Player,
    Squad,
    Friendly,
    Neutral,
    Enemy,
}


[System.Serializable]
public enum DialogueType{
        Dialogue,
        InterruptedDialogue,
        Interruption,
        Prompt,
        Choice,
        Action,
}

[CreateAssetMenu(fileName = "New Speaker", menuName = "Dialogue/Speaker")]
public class Speaker : ScriptableObject {
    public string speakerName;
    public Sprite image;
    public SpeakerType type;
    public Faction faction;
}

[System.Serializable]
public class DialogueLine{
    public Speaker speaker;
    public DialogueType dialogueType;
    public string line;
}

