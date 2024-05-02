using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

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
    Enemy_Spawner enemySpawner;
    GameObject formationPoint;


    [SerializeField] DialogueManager dialogue;
    [SerializeField] DialogueScriptableObject script;

    Plane playerPlane;
    Autopilot ap;
    PlayerWeaponManager playerWeapons;

   void OnEnable(){
        DialogueManager.OnRequestPitchControl += AxisControlTask;
   } 

    // Start is called before the first frame update
    void Start()
    {
        playSpaceBoundary = GetComponentInChildren<PlaySpaceBoundary>();
        playSpaceBoundary.enforceBoundary = false;

        enemySpawner = GetComponentInChildren<Enemy_Spawner>();
        enemySpawner.isEnabled = false;

        formationPoint = GameObject.Find("FormationPoint");

        playerPlane = GameObject.FindWithTag("Player").GetComponent<Plane>();
        playerWeapons = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerWeaponManager>();
        playerWeapons.isEnabled = false;
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
    }

    void AxisControlTask(TutorialTask task, DialogueManager requester){
        ShowHint();
        Func<bool> CompletionRequirements = null;
        switch (task)
        {
            case TutorialTask.VerticalControls:
                //bitwise and vertical and throttle channels
                playerPlane.EnableChannel(Plane.ControlChannels.Vertical | Plane.ControlChannels.Throttle);
                ap.setAPState(Autopilot.AutopilotState.targetFlat);
                CompletionRequirements = () => Input.GetAxis("P1_Vertical") > 0;
                break;
            case TutorialTask.HorizontalControls:
                playerPlane.EnableChannel(Plane.ControlChannels.Horizontal | Plane.ControlChannels.Throttle);
                CompletionRequirements = () => Input.GetAxis("P1_Horizontal") > 0;
                break;

            case TutorialTask.Boost:
                playerPlane.EnableAllChannels();
                CompletionRequirements = () => Input.GetAxis("P1_Throttle") > 0;
                break;
            case TutorialTask.Boundary:
                playSpaceBoundary.enforceBoundary = true;
                break;
            case TutorialTask.PrimaryFire:
                playerWeapons.enabled = true;
                break;
            case TutorialTask.SecondaryFire:
                playerWeapons.enabled = true;
                break;
            default:
                //Always exit as if task completed if no task is set
                CompletionRequirements = () => true;
                break;
        }
        StartCoroutine(WaitForTask(CompletionRequirements, requester));
    }

    void ShowHint(){
        Debug.Log("Show Hint");
    }


    IEnumerator WaitForTask(Func<bool> Req, DialogueManager requester){
        yield return new WaitUntil(() => Req());
        {
            Debug.Log("Vertical Control Task Complete");
            StartCoroutine(OnCompleteTask(requester));
        }
    }

    IEnumerator OnCompleteTask(DialogueManager requester){
        yield return new WaitForSeconds(1);
        playerPlane.DisableAllChannels();
        playerWeapons.enabled = false;
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        requester.SetTaskComplete();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
