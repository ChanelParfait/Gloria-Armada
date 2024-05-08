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

    [SerializeField] bool useLERP = false;
    [SerializeField] bool isRotating = false;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject playerPlane;

    // UI and Visuals
    [SerializeField] private GameObject gameOverPnl; 
    [SerializeField] private GameObject youWinPnl; 
    [SerializeField] private TextMeshProUGUI ScoreTxt; 
    public Animator damageAnim;

    public List<GameObject> enemies = new List<GameObject>();
    public bool spawnOverTime = false;

    float lastSpawnTime = 0;
    float spawnInterval = 5.0f;


    // UI Values
    private int score = 0; 

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
        gameOverPnl = GameObject.Find("GameOver");
        youWinPnl = GameObject.Find("YouWin");
        UpdatePerspective(initPerspective);
        rb.velocity = Vector3.right * 20;

        if (playerPlane == null)
        {
            //Find the player by tag
            playerPlane = GameObject.FindGameObjectWithTag("Player");
        }

        //This is the minimum velocity to keep the player moving
        //rb.velocity = Vector3.right * 20;
    }

    void FixedUpdate(){
        // Calculate the current distance from the target to the camera's position
        
        // Modify X of target position based on rb velocity between minSpeed and maxSpeed
        if(playerPlane){
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
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnOverTime){
            if (Time.time - lastSpawnTime > spawnInterval){
                lastSpawnTime = Time.time;
                enemySpawner.SpawnEnemy(SpawnPointName.Top_Right, UnityEngine.Random.Range(0, 2));
            }
        }
    }

    private void OnEnable(){
        // Update Score on enemy death 
        EnemyBase.OnEnemyDeath += UpdateScore;
        PlayerPlane.OnPlayerDeath += GameOver;
        PlayerPlane.OnPlayerDamage += PlayDamageEffect;

    }

    private void OnDisable(){
        // if gameobject is disabled remove all listeners
        EnemyBase.OnEnemyDeath -= UpdateScore;
        PlayerPlane.OnPlayerDeath -= GameOver;
        PlayerPlane.OnPlayerDamage -= PlayDamageEffect;
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            UpdatePerspective(col.GetComponent<TransitionPoint>().GetPerspective());
        }
        if(col.tag == "WinPoint"){
            YouWin();
        }
    }

    public static Vector3 PerspectiveToEuler(Perspective p){
        //Convert perspective to euler angles
        switch (p){
            case Perspective.Side_On:
                return new Vector3(0, 0, 0);
            case Perspective.Top_Down:
                return new Vector3(90, 0, -90);
            default:
                Transform camDirector = GameObject.Find("CameraDirector").transform;
                return camDirector.rotation.eulerAngles;
        }
    }

    public void UpdatePerspective(Perspective pers){
        Vector3 currentOrientation = PerspectiveToEuler(currentPerspective);
        currentPerspective = pers; 

        if (anim != null){
            anim.SetInteger("Perspective", (int)currentPerspective);
            OnPerspectiveChange?.Invoke((int)currentPerspective);
        }
        else if (useLERP && !isRotating){
            
            Vector3 newOrientation = PerspectiveToEuler(pers);
            Transform camDirector = transform.Find("CameraDirector");

            StartCoroutine(LerpOrientation(4f, currentOrientation, newOrientation, camDirector));
            OnPerspectiveChange?.Invoke((int)currentPerspective);
        }
        //jetControl.ResetPosition(5f);
        //Invoke action to update others without storing references to all objects
        
    }

    IEnumerator LerpOrientation(float time, Vector3 current, Vector3 target, Transform cam){
        //Get CameraDirector child
        isRotating = true;
        float elapsedTime = 0;
        while (elapsedTime < time){
            float t = elapsedTime / time;
            cam.rotation = Quaternion.Euler(Vector3.Lerp(current, target, Utilities.EaseInOut(t)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isRotating = false;
        cam.rotation = Quaternion.Euler(target);
    }

    private void UpdateScore(EnemyBase enemy){
        Debug.Log("Update Score");
        if(ScoreTxt){
            score += enemy.scoreValue;
            ScoreTxt.text = score.ToString();
        }
    }

    private void PlayDamageEffect(PlayerPlane playerPlane){
        damageAnim.SetTrigger("DamageTaken");
    }

    private void GameOver(){
        GameObject wreckage = GameObject.FindWithTag("PlayerWreckage");
        //Pick a random child from player wreckage
        Transform randomChild = wreckage.transform.GetChild(0).GetChild(UnityEngine.Random.Range(0, wreckage.transform.childCount));
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        string playerName = PlayerPrefs.GetString("PlayerName");
        string scene_player = sceneName + "_" + playerName;
        PlayerPrefs.SetInt(scene_player, score);

        StartCoroutine(ShowDeathScreen(randomChild));
    }

    IEnumerator ShowDeathScreen(Transform wreckage){
        GameOverMenu gm = gameOverPnl.GetComponent<GameOverMenu>();
        if (wreckage != null)
        {
            Debug.Log("Waiting for wreck to settle");
            Func<bool> Req = () => wreckage.GetComponent<Rigidbody>().velocity.magnitude == .0f;
            yield return StartCoroutine(WaitOrSkip(10.0f, Req));
            gameOverPnl.SetActive(true);
            gameOverPnl.GetComponent<GameOverMenu>().timerStart = true;

        }
    }

    IEnumerator WaitOrSkip(float waitTime, Func<bool> skipRequirement){
        float t = 0;
        while (t < waitTime){
            // Check if the space key is pressed
            if (skipRequirement() && t > 2.0f)
            {
                Debug.Log("Requirement Met - skipping");
                yield break;  // Exit the coroutine early
            }
            yield return null;
            t += Time.deltaTime;
            // Wait for the next frame   
        }
    }

    public void YouWin(){
        //youWinPnl.SetActive(true);
        StartCoroutine(goToNextLevel());
    }

    IEnumerator goToNextLevel(){
        yield return new WaitForSeconds(3);
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Restart(){
        SceneManager.LoadScene(1);
        Time.timeScale = 1; 
    }

}
