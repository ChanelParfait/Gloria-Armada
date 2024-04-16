using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Perspective {Null = 0, Side_On = 1, Top_Down = 2};

public class LevelManager : MonoBehaviour
{
    // Keep track of level perspective and update all other objects 
    [SerializeField] Perspective initPerspective; 
    private Perspective currentPerspective; 
    [SerializeField] private Animator anim; 

    [SerializeField] private Enemy_Spawner enemySpawner;
    [SerializeField] private JetControl jetControl;

    [SerializeField] private GameObject playerPlane;

    [SerializeField] private GameObject gameOverPnl; 
    [SerializeField] private GameObject youWinPnl; 
    [SerializeField] private Text CurrentHealthTxt; 
    private int playerHealth; 

    public static event Action<int> OnPerspectiveChange;

    bool isGameOver = false;

    void Awake(){
        currentPerspective = initPerspective;
    }

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        UpdatePerspective(initPerspective);
        enemySpawner.UpdatePerspective(currentPerspective);
        playerHealth = playerPlane.GetComponent<Plane>().health;
    }

    // Update is called once per frame
    void Update()
    {
        // if(jetControl.isDead && !isGameOver){
        //     GameOver();
        //     isGameOver = true;
        // }

        if(isGameOver){
            if(Input.GetKeyDown(KeyCode.Return)){
                Restart();
            }
        }

        // if(playerHealth != jetControl.health){
        //     playerHealth = jetControl.health;
        //     UpdateHealthTxt(playerHealth.ToString());
        // }
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            UpdatePerspective(col.GetComponent<TransitionPoint>().GetPerspective());
        }
        if(col.tag == "WinPoint"){
            YouWin();
        }
    }

    private void UpdatePerspective(Perspective pers){
        currentPerspective = pers; 
        anim.SetInteger("Perspective", (int)currentPerspective);

        enemySpawner.UpdatePerspective(currentPerspective); 
        //jetControl.ResetPosition(5f);
        //Invoke action to update others without storing references to all objects
        OnPerspectiveChange?.Invoke((int)currentPerspective);
    }

    //public delegate void OnPerspectiveChange(Perspective pers);

    private void UpdateHealthTxt(string health){
        CurrentHealthTxt.text = health;
    }

    private void GameOver(){
        //Debug.Log("Game Over");
        gameOverPnl.SetActive(true);
        Time.timeScale = 0; 
    }

    private void YouWin(){
        youWinPnl.SetActive(true);
        Time.timeScale = 0; 
    }

    private void Restart(){
        SceneManager.LoadScene(1);
        Time.timeScale = 1; 
    }

}
