using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public enum TutorialTask{
        VerticalControls,
        HorizontalControls,
        Boost,
        Boundary,
        PrimaryFire,
        SecondaryFire,
    }
public class TutorialSequence : MonoBehaviour
{
    PlaySpaceBoundary playSpaceBoundary;
    EnemySpawner enemySpawner;
    GameObject formationPoint;

    [SerializeField] Canvas hintCanvas;


    DialogueManager dialogue;
    [SerializeField] DialogueScriptableObject script;

    Plane playerPlane;
    Autopilot ap;
    PlayerWeaponManager playerWeapons;

    bool isEnemyDead = false;

   void OnEnable(){
        DialogueManager.OnRequestPlayerAction += PlayerTask;
        EnemyBase.OnEnemyDeath += EnemyKilled;
   } 

    // Start is called before the first frame update
    void Start()
    {
        playSpaceBoundary = GetComponentInChildren<PlaySpaceBoundary>();
        playSpaceBoundary.enforceBoundary = false;

        enemySpawner = GetComponentInChildren<EnemySpawner>();
        enemySpawner.isEnabled = false;

        formationPoint = GameObject.Find("FormationPoint");

        playerPlane = GameObject.FindWithTag("Player").GetComponent<Plane>();
        playerWeapons = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerWeaponManager>();
        hintCanvas = GameObject.Find("PlayerPhys/Hints/Canvas").GetComponent<Canvas>();
        playerWeapons.isArmed = false;
        ap = GameObject.FindWithTag("Player").GetComponent<Autopilot>();
        dialogue = GetComponent<DialogueManager>();
        dialogue.script = script;
        StartCoroutine(Initialize());
    }

    
    IEnumerator Initialize(){
        yield return new WaitForSeconds(1);
        Debug.Log("Tutorial Sequence Started");
        playerPlane.DisableAllChannels();
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        dialogue.StartDialogue();
        hintCanvas.enabled = false;
    }

    void PlayerTask(TutorialTask task, DialogueManager requester){
        Debug.Log("Task Requested" + task);
        Func<bool> CompletionRequirements = null;
        switch (task)
        {
            case TutorialTask.VerticalControls:
                //bitwise and vertical and throttle channels
                ShowHint("W");
                playerPlane.EnableChannel(Plane.ControlChannels.Vertical | Plane.ControlChannels.Throttle);
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                CompletionRequirements = () => Input.GetAxis("P1_Vertical") < 0;
                break;
            case TutorialTask.HorizontalControls:
                ShowHint("A");
                playerPlane.EnableChannel(Plane.ControlChannels.Horizontal | Plane.ControlChannels.Throttle);
                CompletionRequirements = () => Input.GetAxis("P1_Horizontal") > 0;
                break;

            case TutorialTask.Boost:
                ShowHint("Shift");
                playerPlane.EnableAllChannels();
                CompletionRequirements = () => Input.GetKeyDown(KeyCode.LeftShift);
                break;
            case TutorialTask.Boundary:
                playSpaceBoundary.enforceBoundary = true;
                break;
            case TutorialTask.PrimaryFire:
                ShowHint("Space");
                playerPlane.EnableChannel(Plane.ControlChannels.Vertical | Plane.ControlChannels.Throttle);
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                isEnemyDead = false;
                playerWeapons.isArmed = true;
                CompletionRequirements = () => isEnemyDead == true; 
                isEnemyDead = false;
                break;
            case TutorialTask.SecondaryFire:
                ShowHint("E");
                playerPlane.EnableChannel(Plane.ControlChannels.Vertical | Plane.ControlChannels.Throttle);
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                isEnemyDead = false;
                playerWeapons.isArmed = true;
                CompletionRequirements = () => isEnemyDead == true; 
                isEnemyDead = false;
                break;
            default:
                //Always exit as if task completed if no task is set
                CompletionRequirements = () => true;
                break;
        }
        hintCanvas.enabled = true;
        StartCoroutine(WaitForTask(CompletionRequirements, requester));
    }

    void ShowHint(string key){
        hintCanvas.transform.Find("HintText").GetComponent<TextMeshProUGUI>().text = key;   
        hintCanvas.enabled = true;
        StartCoroutine(HintDisplay(key));
    }

    IEnumerator HintDisplay(string key){    
        hintCanvas.transform.Find("HintText").GetComponent<TextMeshProUGUI>().text = key;   
        yield return new WaitForSeconds(1);
        hintCanvas.enabled = false;
    }


    IEnumerator WaitForTask(Func<bool> Req, DialogueManager requester){
        yield return new WaitUntil(() => Req());
        {
            Debug.Log("Task Complete");
            StartCoroutine(OnCompleteTask(requester));
        }
    }

    void EnemyKilled(EnemyBase enemy){
        isEnemyDead = true;
    }

    IEnumerator OnCompleteTask(DialogueManager requester){
        yield return new WaitForSeconds(1.5f);
        playerPlane.DisableAllChannels();
        playerWeapons.isArmed = false;
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        requester.SetTaskComplete();
        hintCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
            LevelManager lm  = GetComponent<LevelManager>();
            lm.YouWin();
        }
    }
}
