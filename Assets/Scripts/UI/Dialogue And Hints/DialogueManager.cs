using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class DialogueManager : MonoBehaviour
{

    public DialogueScriptableObject script;

    public List<DialogueLine> currentDialogue;

    private int index;
    private int typeSpeed = 50;
    private int defaultSpeed = 50;
    private int fastSpeed = 1;

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

    Canvas choicePrompts;
    RectTransform choiceYes;
    RectTransform choiceNo;

    public static UnityAction<TutorialTask, DialogueManager> OnRequestPlayerAction;

    bool taskComplete = false;
    bool choiceMade = false;
    bool isResponseYes = false;
    bool repeatLine = false;

    void OnEnable(){

    }

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
        choicePrompts = GameObject.Find("ChoicePrompts").GetComponent<Canvas>();
        choiceYes = choicePrompts.transform.Find("Mask/Yes_Mask/Yes_Prompt").GetComponent<RectTransform>();
        choiceNo = choicePrompts.transform.Find("Mask/No_Mask/No_Prompt").GetComponent<RectTransform>();


        promptContinueDialog.anchoredPosition = new Vector2(1000, 0);
        choiceYes.anchoredPosition = new Vector2(0, -1000);
        choiceNo.anchoredPosition = new Vector2(0, -1000);
        
        //Initialize the dialogue line
        currentDialogue = script.lines;
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

        //Display the line for a second before moving on to the next line
        //Debug.Log("Line Length: " + tutorial[index].line.Length + " Typed chars: " + currentDialogueText.text.Length);   
        if (currentDialogueText.text.Length == currentDialogue[index].line.Length){
            //Debug.Log("Line typed" + ", DialogueType: " + currentDialogue[index].dialogueType);
            switch (currentDialogue[index].dialogueType){
                case DialogueType.Dialogue:
                    StartCoroutine(WaitOrSkip(3));
                    break;
                case DialogueType.InterruptedDialogue:
                    break;
                case DialogueType.Choice:
                    choicePrompts.enabled = true;
                    choiceYes.Find("Yes_Text").GetComponent<TextMeshProUGUI>().text = currentDialogue[index].choice.affirm.optionText;
                    choiceNo.Find("No_Text").GetComponent<TextMeshProUGUI>().text = currentDialogue[index].choice.negate.optionText;
                    foreach (RectTransform rect in new RectTransform[] {choiceYes, choiceNo}){                        
                        StartCoroutine(WipeInRect(rect, rect.anchoredPosition, true));          
                    }
                    StartCoroutine(AwaitChoice());
                    yield return new WaitUntil(() => choiceMade);
                    choiceMade = false; //Reset the choiceMade flag
                    if (isResponseYes){
                        //ToDo: Unpack the choice and execute the appropriate action
                        //currentDialogue[index].choice.affirm.action();
                        StartCoroutine(BounceOut(choiceYes));
                        StartCoroutine(WipeOutRect(choiceNo));
                    }
                    else{
                        //ToDo: Unpack the choice and execute the appropriate action
                        StartCoroutine(BounceOut(choiceNo));
                        StartCoroutine(WipeOutRect(choiceYes));
                    }
                    ExecuteChoice(currentDialogue[index].choice, isResponseYes);
                    yield return new WaitForSeconds(1);
                    choicePrompts.enabled = false;
                    break;

                case DialogueType.Prompt:
                    promptContinue.enabled = true;
                    StartCoroutine(WipeInRect(promptContinueDialog, promptContinueDialog.anchoredPosition, true));
                    //Wait for the player to press the continue button
                    yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                    StartCoroutine(WipeOutRect(promptContinueDialog));
                    promptContinue.enabled = false;
                    break;
                case DialogueType.Action:
                    OnRequestPlayerAction?.Invoke(currentDialogue[index].tutorialTask, this);
                    //Wait for the task to be completed
                    yield return new WaitUntil(() => taskComplete);
                    taskComplete = false;
                    StartCoroutine(WipeOutRect(promptContinueDialog));
                    break;
                case DialogueType.Event:
                    currentDialogue[index].dialogueEvent.DoEvent();
                    break;
            }
            if (currentDialogue[index].dialogueType == DialogueType.Dialogue){
                yield return new WaitForSeconds(0.5f);
            }          
            NPCDialogueText.text = "";
            NextLine();
        }   
    }

    IEnumerator WaitOrSkip(float waitTime){
        float t = 0;
        while (t < waitTime){
            // Check if the space key is pressed
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("Space key pressed. Skipping wait time.");
                yield break;  // Exit the coroutine early
            }
            yield return null;
            t += Time.deltaTime;
            // Wait for the next frame   
        }
    }

    public void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());
    }

    public void StartDialogue(DialogueScriptableObject _script){
        script = _script;
        currentDialogue = script.lines;
        index = 0;
        StartCoroutine(TypeLine());
    }

    public void StartDialogue(DialogueScriptableObject _script, int _index){
        script = _script;
        currentDialogue = script.lines;
        index = _index;
        StartCoroutine(TypeLine());
    }
    public void StartDialogue(int index){
        this.index = index;
        StartCoroutine(TypeLine());
    }   

    private void NextLine(){
        typeSpeed = defaultSpeed;
        if (repeatLine){
            repeatLine = false;
            StartCoroutine(TypeLine());
        }
        else {
            index++;
            if (index < currentDialogue.Count){
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

    }

    public void SetTaskComplete(){
        taskComplete = true;
    }

        // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame){
            typeSpeed = fastSpeed;
        } 

        //Get choice from player (lasts one frame)
        if (choiceMade == true){
            choiceMade = false;
        }
        if (Input.GetButtonDown("P1_Fire1")) {
            isResponseYes = true;  
            choiceMade = true;
        }
        if (Input.GetButtonDown("P1_Fire2")) {
            isResponseYes = false; 
            choiceMade = true;
        }
    }

    IEnumerator AwaitChoice(){
        if (Keyboard.current.yKey.wasPressedThisFrame){
            choiceMade = true;
            isResponseYes = true;
        }
        else if (Keyboard.current.nKey.wasPressedThisFrame){
            choiceMade = true;
            isResponseYes = false;
        }
        if (choiceMade){
            yield return null;
        }
    }

    void ExecuteChoice(DialogueChoice choice, bool _isResponseYes){
        ChoiceOption chosen = _isResponseYes ? choice.affirm : choice.negate;
        ChoiceType choiceType = chosen.choiceType;
        switch (choiceType){
            case ChoiceType.Default:
                break;
            case ChoiceType.Repeat:
                repeatLine = true;
                break;
            case ChoiceType.ChangeDialogue:
                currentDialogue = chosen.newDialogue.lines;
                index = chosen.newDialogueIndex;
                break;
            case ChoiceType.EndDialogue:
                break;
        }
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

    IEnumerator WipeInRect(RectTransform rect, Vector2 initPosition, bool wipeIn = true){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        Vector2 fromPosition = wipeIn ? initPosition : new Vector2(0, 0);
        Vector2 toPosition = wipeIn ? new Vector2(0, 0) : initPosition;
        rect.anchoredPosition = fromPosition;
        rect.localRotation = wipeIn ? Quaternion.Euler(0, 0, 30): Quaternion.Euler(0, 0, 0);
        float t = 0;
        while (t < 1){
            t += 2 * Time.deltaTime;
            rect.anchoredPosition = new Vector2(Mathf.LerpUnclamped(fromPosition.x, toPosition.x, Utilities.EaseInOutBack(t)),
                                                Mathf.LerpUnclamped(fromPosition.x, toPosition.x, Utilities.EaseInOutBack(t)));
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

    IEnumerator BounceOut(RectTransform rect){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        rect.anchoredPosition = new Vector2(0, 0);
        rect.localRotation = Quaternion.Euler(0, 0, 0);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            rect.anchoredPosition = new Vector3(0, Mathf.LerpUnclamped(0, 2, Utilities.EaseInOutBack(t)), 0);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(0, 5, Utilities.EaseInOutBack(t)));
            rect.localScale = new Vector3(1, Mathf.LerpUnclamped(1, 1.4f, Utilities.EaseInOutBack(t)), 1);
            yield return null;
        }
        StartCoroutine(BounceIn(rect));
    }
    IEnumerator BounceIn(RectTransform rect){
        // Take a canvas and start it slightly below size, at an angle then LERP to it being full size, horizontal
        rect.anchoredPosition = new Vector3(0, 2);
        rect.localRotation = Quaternion.Euler(0, 0, 5);
        float t = 0;
        while (t < 1){
            t += Time.deltaTime;
            rect.anchoredPosition = new Vector3(0, Mathf.LerpUnclamped(10, 0, Utilities.EaseInOutBack(t)), 0);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(5, 0, Utilities.EaseInOutBack(t)));
            rect.localScale = new Vector3(1, Mathf.LerpUnclamped(1.4f, 1, Utilities.EaseInOutBack(t)), 1);
            yield return null;
        }
        StartCoroutine(WipeOutRect(rect));
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
}
