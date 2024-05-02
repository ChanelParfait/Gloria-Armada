using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;



public class DialogueManager : MonoBehaviour
{
    public class DialogueAction{
        InputAction action;
    }   

    public enum DialogueType{
        Dialogue,
        InterruptedDialogue,
        Interruption,
        Prompt,
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

    public static DialogueLine[] currentDialogue = new DialogueLine[] {
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Prompt,
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
        new DialogueLine {
            speaker = instructor,
            type = DialogueType.Action,
            line = "I want you to pitch up and down to follow me"
        },
        // Additional dialogue lines can be added here
    };

    private int index;
    private int typeSpeed = 50; //In characters per second
    // private int defaultSpeed = 50;
    // private int fastSpeed = 150;

    Canvas NPCDialogueCanvas;
    public Transform NPCBackground;
    public TextMeshProUGUI NPCNameText;
    public TextMeshProUGUI NPCDialogueText;

    class DialogueBox{
        public Canvas canvas;
        public Image background;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
    }

    [SerializeField] DialogueBox NPCDialogueBox = new DialogueBox();
    [SerializeField] DialogueBox playerDialogueBox = new DialogueBox();

    DialogueBox currentDialogueBox;

    DialogueBox[] dialogueBoxes;

    [SerializeField] Canvas playerDialogueCanvas;
    public Transform playerBackground;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerDialogueText;

    Canvas currentCanvas;
    Transform currentBackground;
    TextMeshProUGUI currentNameText;
    TextMeshProUGUI currentDialogueText;

    Canvas promptContinue;
    RectTransform promptContinueDialog;

    public static UnityAction<TutorialTask, DialogueManager> OnRequestPitchControl;
    public static UnityAction<TutorialTask, DialogueManager> OnRequestHorizontalControl;
    public static UnityAction<TutorialTask, DialogueManager> OnRequestShoot;
    public static UnityAction<TutorialTask, DialogueManager> OnRequestShootSpecial;

    bool taskComplete = false;



    // Start is called before the first frame update
    void Start()
    {
        NPCDialogueCanvas = GameObject.Find("ConversationL").GetComponent<Canvas>();
        NPCBackground = NPCDialogueCanvas.transform.Find("Background");
        NPCDialogueText = NPCDialogueCanvas.transform.Find("Background/Dialogue").GetComponent<TextMeshProUGUI>();
        NPCNameText = NPCDialogueCanvas.transform.Find("Background/CharacterPicture/CharacterName").GetComponent<TextMeshProUGUI>();
        playerDialogueCanvas = GameObject.Find("ConversationR").GetComponent<Canvas>();

        playerDialogueText = playerDialogueCanvas.transform.Find("Background/Dialogue").GetComponent<TextMeshProUGUI>();
        playerNameText = playerDialogueCanvas.transform.Find("Background/CharacterPicture/CharacterName").GetComponent<TextMeshProUGUI>();
        playerBackground = playerDialogueCanvas.transform.Find("Background");
        promptContinue = GameObject.Find("PromptContinue").GetComponent<Canvas>();
        promptContinueDialog = promptContinue.transform.Find("Mask/PromptDescription").GetComponent<RectTransform>();

        promptContinueDialog.anchoredPosition = new Vector2(1000, 0);
    }

    IEnumerator TypeLine(){
        // Set the name of the speaker
        currentCanvas = currentDialogue[index].speaker.type == SpeakerType.NPC ? NPCDialogueCanvas : playerDialogueCanvas;
        currentBackground = currentDialogue[index].speaker.type == SpeakerType.NPC ? NPCBackground : playerBackground;
        currentNameText = currentDialogue[index].speaker.type == SpeakerType.NPC ? NPCNameText : playerNameText;
        currentDialogueText = currentDialogue[index].speaker.type == SpeakerType.NPC ? NPCDialogueText : playerDialogueText;

        currentNameText.text = currentDialogue[index].speaker.name;
        currentDialogueText.text = "";

        if (currentCanvas.enabled == false){
            currentCanvas.enabled = true;
            StartCoroutine(PopInBox(currentCanvas, currentBackground));
        }

        foreach( char c in currentDialogue[index].line.ToCharArray()){
            currentDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed/1000f);
        }
        // when line is done typing, check if there is a choice or action to be made
        // TODO

        //Display the line for a second before moving on to the next line
        //Debug.Log("Line Length: " + tutorial[index].line.Length + " Typed chars: " + currentDialogueText.text.Length);   
        if (currentDialogueText.text.Length == currentDialogue[index].line.Length){
            Debug.Log("Line typed");
            switch (currentDialogue[index].type){
                case DialogueType.Dialogue:
                    yield return new WaitForSeconds(3);
                    break;
                case DialogueType.InterruptedDialogue:
                    break;
                case DialogueType.Choice:
                    //Display the choices
                    break;

                case DialogueType.Prompt:
                    StartCoroutine(WipeInBox(promptContinue, promptContinueDialog));
                    //Wait for the player to press the continue button
                    yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                    StartCoroutine(WipeOutRect(promptContinueDialog));
                    break;
                case DialogueType.Action:
                    OnRequestPitchControl?.Invoke(TutorialTask.VerticalControls, this);
                    //Wait for the task to be completed
                    yield return new WaitUntil(() => taskComplete);
                    taskComplete = false;
                    StartCoroutine(WipeOutRect(promptContinueDialog));
                    break;
            }
            if (currentDialogue[index].type == DialogueType.Dialogue){
                yield return new WaitForSeconds(1);
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
        if (index < currentDialogue.Length){
            //If the next speaker differs from the current speaker then disable the current canvas
            if (currentDialogue[index].speaker.type != currentDialogue[index-1].speaker.type){
                StartCoroutine(PopOutBox(currentCanvas, currentBackground));
            }
            StartCoroutine(TypeLine());
        }
        else{
            //End of dialogue
            currentCanvas.enabled = false;
        }
    }

    public void SetTaskComplete(){
        taskComplete = true;
    }

    IEnumerator PopInBox(Canvas canvas, Transform background){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        canvas.enabled = true;
        canvas.scaleFactor = 0.7f;
        canvas.transform.rotation = Quaternion.Euler(0, 0, 45);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            canvas.scaleFactor = Mathf.LerpUnclamped(0.7f, 1, Utilities.EaseInOutBack(t));
            canvas.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(45, 0, Utilities.EaseInOutBack(t)));
            yield return null;
        }
    }

    IEnumerator PopOutBox(Canvas canvas, Transform background){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        canvas.scaleFactor = 1;
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            canvas.scaleFactor = Mathf.LerpUnclamped(1, 0.7f, Utilities.EaseInOutBack(t));
            canvas.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(0, 45, Utilities.EaseInOutBack(t)));
            yield return null;
        }
        canvas.enabled = false;
    }   

    IEnumerator WipeInBox(Canvas canvas, RectTransform rect){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        rect.anchoredPosition = new Vector3(1000, 0, 0);
        rect.localRotation = Quaternion.Euler(0, 0, 30);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            rect.anchoredPosition = new Vector3(Mathf.LerpUnclamped(1000, 0, Utilities.EaseInOutBack(t)), 0, 0);
            rect.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(30, 0, Utilities.EaseInOutBack(t)));
            yield return null;
        }
    }

    IEnumerator WipeOutRect(RectTransform rect){
        rect.anchoredPosition = new Vector2(0, 0);
        rect.localRotation = Quaternion.Euler(0, 0, 0);
        float t = 0;
        while (t < 1){
            t += 2* Time.deltaTime;
            rect.anchoredPosition = new Vector2(Mathf.LerpUnclamped(0, 1000, Utilities.EaseInOutBack(t)), 0);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(0, 30, Utilities.EaseInOutBack(t)));
            yield return null;
        }
    }

    IEnumerator Bounce(Canvas canvas, RectTransform rect){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        rect.anchoredPosition = new Vector3(0, 0, 0);
        rect.localRotation = Quaternion.Euler(0, 0, 0);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            rect.anchoredPosition = new Vector3(0, Mathf.LerpUnclamped(0, 10, Utilities.EaseInOutBack(t)), 0);
            yield return null;
        }
    }

    IEnumerator PopInInterrupt(Canvas canvas, Transform background){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        canvas.scaleFactor = 0.7f;
        background.rotation = Quaternion.Euler(0, 0, 45);
        float t = 0;
        while (t < 1){
            t += 3 * Time.deltaTime;
            canvas.scaleFactor = Mathf.LerpUnclamped(0.7f, 1, Utilities.EaseInOutBack(t));
            canvas.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(45, 0, Utilities.EaseInOutBack(t)));
            yield return null;
        }
    }

    IEnumerator PopOutInterrupt(Canvas canvas, Transform background){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        canvas.scaleFactor = 0.7f;
        background.rotation = Quaternion.Euler(0, 0, 45);
        float t = 0;
        while (t < 1){
            t += 3 * Time.deltaTime;
            canvas.scaleFactor = Mathf.LerpUnclamped(0.7f, 1, Utilities.EaseInOutBack(t));
            canvas.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(45, 0, Utilities.EaseInOutBack(t)));
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
