using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;



public class Dialogue : MonoBehaviour
{
    public class DialogueAction{
        InputAction action;
    }   

    public enum DialogueType{
        Dialogue,
        InterruptedDialogue,
        Choice,
        Action,
    }

    public enum SpeakerType{
        Player,
        NPC
    }

    public class Speaker {
        public string name;
        public Image image;
        public SpeakerType type;
    }

    public struct DialogueLine{
        public Speaker speaker;
        public DialogueType type;
        public string line;
    }

    private static Speaker instructor = new()
    {
        name = "Instructor",
        type = SpeakerType.NPC
    };

    private static Speaker player = new()
    {
        name = "Player",
        type = SpeakerType.Player
    };

    private static DialogueLine[] tutorial = new DialogueLine[] {
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "Ok, test pilot, comms check. Do you read me?"
        },
        new DialogueLine {
            speaker = player,
            type = DialogueType.Dialogue,
            line = "I read you loud and clear"
        },
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "Good. Now, let's get started."
        },
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "I've set your plane's autopilot to hold formation with me"
        },
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "It's still an experimental aircraft, so be careful"
        },
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "You don't want to end up like the last pilot"
        },
        new DialogueLine {
            speaker = player,
            type = DialogueType.InterruptedDialogue,
            line = "What happened to the last-"
        },
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Dialogue,
            line = "Ok first line item: Pitch response"
        },
        // Additional dialogue lines can be added here
    };

    private int index;
    private int typeSpeed = 50; //In characters per second
    private int defaultSpeed = 50;
    private int fastSpeed = 150;

    [SerializeField] DialogueLine[] dialogue;

    [SerializeField] Canvas NPCDialogueCanvas;
    public TextMeshProUGUI NPCNameText;
    public TextMeshProUGUI NPCDialogueText;

    [SerializeField] Canvas playerDialogueCanvas;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerDialogueText;

    Canvas currentCanvas;
    TextMeshProUGUI currentNameText;
    TextMeshProUGUI currentDialogueText;



    // Start is called before the first frame update
    void Start()
    {
        NPCDialogueCanvas = GameObject.Find("ConversationL").GetComponent<Canvas>();
        playerDialogueCanvas = GameObject.Find("ConversationR").GetComponent<Canvas>();
        NPCDialogueText = NPCDialogueCanvas.transform.Find("Background/Dialogue").GetComponent<TextMeshProUGUI>();
        NPCNameText = NPCDialogueCanvas.transform.Find("Background/CharacterPicture/CharacterName").GetComponent<TextMeshProUGUI>();
        playerDialogueText = playerDialogueCanvas.GetComponentInChildren<TextMeshProUGUI>();
        playerNameText = playerDialogueCanvas.GetComponentInChildren<TextMeshProUGUI>();
        StartDialogue();
    }

    IEnumerator TypeLine(){
        // Set the name of the speaker
        currentCanvas = tutorial[index].speaker.type == SpeakerType.NPC ? NPCDialogueCanvas : playerDialogueCanvas;
        currentNameText = tutorial[index].speaker.type == SpeakerType.NPC ? NPCNameText : playerNameText;
        currentDialogueText = tutorial[index].speaker.type == SpeakerType.NPC ? NPCDialogueText : playerDialogueText;

        currentNameText.text = tutorial[index].speaker.name;
        currentDialogueText.text = "";

        if (currentCanvas.enabled == false){
            currentCanvas.enabled = true;
        }

        foreach( char c in tutorial[index].line.ToCharArray()){
            currentDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed/1000f);
        }
        // when line is done typing, check if there is a choice or action to be made
        // TODO

        //Display the line for a second before moving on to the next line
        //Debug.Log("Line Length: " + tutorial[index].line.Length + " Typed chars: " + currentDialogueText.text.Length);   
        if (currentDialogueText.text.Length == tutorial[index].line.Length){
            Debug.Log("Line typed");
            if (tutorial[index].type == DialogueType.Dialogue){
                yield return new WaitForSeconds(3);
            }          
            NPCDialogueText.text = "";
            NextLine();
        }

        
    }

    public void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());
    }

    private void NextLine(){
        index++;
        if (index < tutorial.Length){
            //If the next speaker differs from the current speaker then disable the current canvas
            if (tutorial[index].speaker.type != tutorial[index-1].speaker.type){
                currentCanvas.enabled = false;
            }
            StartCoroutine(TypeLine());
        }
        else{
            //End of dialogue
            currentCanvas.enabled = false;
        }
    }



    // Update is called once per frame
    void Update()
    {
    }
}
