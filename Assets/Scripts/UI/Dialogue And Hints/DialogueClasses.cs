using System;
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
public enum ChoiceType
{
    Default,
    Repeat,
    ChangeDialogue,
    EndDialogue,
}

[System.Serializable]
public class ChoiceOption
{
    public string optionText;
    public ChoiceType choiceType;
    public DialogueScriptableObject newDialogue;
    public int newDialogueIndex;
    public Func<bool> action;
}

[System.Serializable]
public class DialogueChoice
{
    public ChoiceOption affirm;
    public ChoiceOption negate;
}


[System.Serializable]
public enum DialogueType{
        Dialogue,
        InterruptedDialogue,
        Interruption,
        Prompt,
        Choice,
        Action,
        Event,
}

[System.Serializable]
public class DialogueEvent
{
    public string eventName;
    public Action action;
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

