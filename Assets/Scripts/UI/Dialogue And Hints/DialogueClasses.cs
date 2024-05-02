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
public class DialogueChoice
{
    public string affirmative = "Yes";
    public string negative = "No";
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



[System.Serializable]
public class DialogueLine{
    public Speaker speaker;
    public DialogueType dialogueType;
    public string line;
    public DialogueChoice choice; // Add this

    public DialogueLine()
    {
        choice = new DialogueChoice();
    }

}

