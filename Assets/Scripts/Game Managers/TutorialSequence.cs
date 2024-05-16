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
        FreeFlight,
        Wait,
    }
public class TutorialSequence : MonoBehaviour
{
    PlaySpaceBoundary playSpaceBoundary;
    EnemySpawner enemySpawner;
    GameObject formationPoint;

    [SerializeField] Canvas hintCanvas;

    CameraDirector camDirector;


    DialogueManager dialogue;
    [SerializeField] DialogueScriptableObject script;

    Plane playerPlane;
    Autopilot ap;
    Autopilot instructorAP;
    PlayerWeaponManager playerWeapons;

    float timer;
    bool isTimerRunning = false;

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
        hintCanvas.enabled = false;
        playerWeapons.isArmed = false;
        ap = GameObject.FindWithTag("Player").GetComponent<Autopilot>();
        instructorAP = GameObject.Find("InstructorPlane").GetComponent<Autopilot>();
        dialogue = GetComponent<DialogueManager>();
        dialogue.script = script;
        camDirector = GameObject.Find("CameraDirector").GetComponent<CameraDirector>();
        StartCoroutine(Initialize());
    }

    
    IEnumerator Initialize(){
        yield return new WaitForSeconds(1);
        Debug.Log("Tutorial Sequence Started");
        playerPlane.DisableAllChannels();
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        instructorAP.setAPState(Autopilot.AutopilotState.targetFormation);
        dialogue.StartDialogue();
        
        StartCoroutine(camDirector.LerpFOV(2f, 23f));
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
                playerPlane.EnableAllChannels();
                CompletionRequirements = () => Input.GetAxis("P1_Horizontal") > 0;
                break;

            case TutorialTask.Boost:
                ShowHint("Shift");
                playerPlane.EnableAllChannels();
                CompletionRequirements = () => playerPlane.throttle == 1.0f && Input.GetAxis("P1_Boost") == 1.0f;
                break;
            case TutorialTask.Boundary:
                playerPlane.EnableAllChannels();
                playSpaceBoundary.enforceBoundary = true;
                break;
            case TutorialTask.PrimaryFire:
                ShowHint("Space");
                playerPlane.EnableAllChannels();
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                isEnemyDead = false;
                playerWeapons.isArmed = true;
                CompletionRequirements = () => isEnemyDead == true; 
                isEnemyDead = false;
                break;
            case TutorialTask.SecondaryFire:
                ShowHint("E");
                playerPlane.EnableAllChannels();
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                isEnemyDead = false;
                playerWeapons.isArmed = true;
                CompletionRequirements = () => isEnemyDead == true; 
                isEnemyDead = false;
                break;
            case TutorialTask.FreeFlight:
                playerPlane.EnableAllChannels();
                playerWeapons.isArmed = true;
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                CompletionRequirements = () => false;
                break;
            case TutorialTask.Wait:
                playerPlane.DisableAllChannels();
                playerWeapons.isArmed = false;
                ap.setAPState(Autopilot.AutopilotState.targetFormation);
                isTimerRunning = true;
                CompletionRequirements = () => timer > 10.0f;
                break;
            default:
                //Always exit as if task completed if no task is set
                CompletionRequirements = () => true;
                break;
        }
        StartCoroutine(WaitForTask(CompletionRequirements, requester));
    }

    void ShowHint(string key){
        hintCanvas.transform.Find("HintText").GetComponent<TextMeshProUGUI>().text = key;   
        hintCanvas.enabled = true;
        StartCoroutine(HintDisplay(key));
    }

    IEnumerator HintDisplay(string key){     
        yield return new WaitForSeconds(3);
        hintCanvas.enabled = false;
    }


    IEnumerator WaitForTask(Func<bool> Req, DialogueManager requester){
        yield return new WaitUntil(() => Req());
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Task Complete");
            StartCoroutine(OnCompleteTask(requester));
        }
    }

    void EnemyKilled(EnemyBase enemy){
        isEnemyDead = true;
    }

    IEnumerator OnCompleteTask(DialogueManager requester){
        yield return new WaitForSeconds(3f);
        Debug.Log("Task Complete: Returning to AP");
        playerPlane.DisableAllChannels();
        playerWeapons.isArmed = false;
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        requester.SetTaskComplete();
        hintCanvas.enabled = false;

        isTimerRunning = false;
        timer = 0.0f;

        LevelManager lm = GetComponent<LevelManager>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy.gameObject);
        }
        lm.spawnOverTime = false;
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
            LevelManager lm  = GetComponent<LevelManager>();
            lm.YouWin();
        }

        if (isTimerRunning){
            timer += Time.deltaTime;
        }
    }
}
