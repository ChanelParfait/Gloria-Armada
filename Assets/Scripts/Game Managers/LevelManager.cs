using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public enum Perspective {Null = 0, Side_On = 1, Top_Down = 2};

public class LevelManager : MonoBehaviour
{
    // Keep track of level perspective and update all other objects 
    [SerializeField] Perspective initPerspective; 
    public Perspective currentPerspective { get; private set;} 
    [SerializeField] private Animator anim; 
    [SerializeField] private Enemy_Spawner enemySpawner;
    [SerializeField] private JetControl jetControl;
    [SerializeField] private GameObject playerPlane;

    // UI and Visuals
    [SerializeField] private GameObject gameOverPnl; 
    [SerializeField] private GameObject youWinPnl; 
    [SerializeField] private Text CurrentHealthTxt; 
    [SerializeField] private TextMeshProUGUI ScoreTxt; 
    public Animator damageCircle;

    // UI Values
    private int score = 0; 
    private int playerHealth; 

    // Camera Controls // 
    Rigidbody rb;
    public float maxDistance = 5.0f; // Distance at which camera starts moving
    public float smoothTime = 0.1f; // Smoother transition time
    public float minHorizontalSpeed = 20.0f; // Minimum horizontal speed
    public float maxHorizontalSpeed = 75.0f; // Maximum horizontal speed
    public float minSpeedXOffset = 43f;
    public float maxHeight = 50.0f;
    public Vector3 velocity = Vector3.zero;

    // Events // 
    public static event Action<int> OnPerspectiveChange;

    //bool isGameOver = false;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        currentPerspective = initPerspective;
    }

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        UpdatePerspective(initPerspective);
        enemySpawner.UpdatePerspective(currentPerspective);
        playerHealth = playerPlane.GetComponent<Plane>().health;
        rb.velocity = Vector3.right * 20;

        //This is the minimum velocity to keep the player moving
        //rb.velocity = Vector3.right * 20;
    }

    void FixedUpdate(){
        // Calculate the current distance from the target to the camera's position
        
        //Modify X of target position based on rb velocity between minSpeed and maxSpeed
        float range = maxHorizontalSpeed - minHorizontalSpeed;
        minSpeedXOffset = -((playerPlane.GetComponent<Plane>().getRBVelocity().x - minHorizontalSpeed)/range - 1)* 30f;
        float yOffset = playerPlane.transform.position.y / 200;

        Vector3 targetPosition = playerPlane.transform.position + new Vector3(minSpeedXOffset, yOffset, 0);   
        Vector3 cameraPosition = transform.position;
        Vector3 offset = targetPosition - cameraPosition;

        // Ensure the camera always moves forwards at a minimum speed
        if (rb.velocity.x < minHorizontalSpeed && offset.x < 0)
        {
            rb.AddForce(rb.velocity - Vector3.right * minHorizontalSpeed);
        }
        // And never goes too fast
        else if (rb.velocity.x > maxHorizontalSpeed && offset.x > 0)
        {
            rb.AddForce(rb.velocity - Vector3.right * maxHorizontalSpeed);
        }
        // Otherwise follow the x position of the player
        else {
            float speed = offset.x > 0 ? offset.x : offset.x * -1;
            speed *= 0.3f;
            rb.AddForce( new Vector3(offset.x, 0, 0) * speed);
        }

        // If player is above 20y, move camera up
        if (rb.position.y > maxHeight)
        {
            rb.AddForce(new Vector3(0, -(float)Math.Pow(rb.position.y - maxHeight, 2)*2 -rb.velocity.y, 0));
        }
        else if (rb.position.y < 0)
        {
            //Add a force upward that is greater when player is lower - resist movement
            rb.AddForce(new Vector3(0, (float)Math.Pow(rb.position.y, 2)*2 - rb.velocity.y , 0));
        }
        else
        {
            float relPos = rb.position.y - playerPlane.transform.position.y;
            float positionSign = Mathf.Sign(relPos);
            float damping = playerPlane.GetComponent<Plane>().getRBVelocity().y - rb.velocity.y;
            damping *= damping*Mathf.Sign(damping);
            rb.AddForce(new Vector3(0, -positionSign * (float)Math.Pow(relPos, 2) + damping, 0));
        }

    }

    // Update is called once per frame
    void Update()
    {
        // if(jetControl.isDead && !isGameOver){
        //     GameOver();
        //     isGameOver = true;
        // }

        // if(playerHealth != jetControl.health){
        //     playerHealth = jetControl.health;
        //     UpdateHealthTxt(playerHealth.ToString());
        // }
    }

    private void OnEnable(){
        // Update Score on enemy death 
        Enemy.OnEnemyDeath += UpdateScore;
        PlayerPlane.OnPlayerDeath += GameOver;

    }

    private void OnDisable(){
        // if gameobject is disabled remove all listeners
        Enemy.OnEnemyDeath -= UpdateScore;
        PlayerPlane.OnPlayerDeath -= GameOver;


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

    private void UpdateHealthTxt(string health){
        CurrentHealthTxt.text = health;
    }

    private void UpdateScore(Enemy enemy){
        Debug.Log("Update Score");
        if(ScoreTxt){
            score += enemy.scoreValue;
            ScoreTxt.text = score.ToString();
        }
    }

    private void PlayDamageEffect(){
        damageCircle.SetTrigger("DamageTaken");
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
