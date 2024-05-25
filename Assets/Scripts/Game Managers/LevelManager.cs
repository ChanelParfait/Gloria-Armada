using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] private GameObject levelClearPnl; 
    [SerializeField] private TextMeshProUGUI ScoreTxt; 
    [SerializeField] private TextMeshProUGUI timerTxt;

    [SerializeField] private GameManager gameManager;
    public Animator damageAnim;

    public List<GameObject> enemies = new List<GameObject>();
    public bool spawnOverTime = false;

    float lastSpawnTime = 0;
    float spawnInterval = 10.0f;

    float levelTimer = 0;


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
        UpdatePerspective(currentPerspective);

        gameManager = GameManager.instance;
        if (gameManager == null){
            GameObject gmObject = new GameObject("GameManager");
            gameManager = gmObject.AddComponent<GameManager>();
        }
    }

    // Set to spawn rate to spawn enemies every 'interval' seconds
    public void SetSpawnInterval(float interval){
        spawnInterval = interval;
    }

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        gameOverPnl = GameObject.Find("GameOver");
        levelClearPnl = GameObject.Find("LevelClear");
        if (levelClearPnl != null){
            EnsureNextLevelButtonListener();
        }
        damageAnim = GameObject.Find("DamageCirclePrefab").GetComponent<Animator>();
        GameObject timerObj = GameObject.Find("Timer");
        if (timerObj != null){
            ScoreTxt = GameObject.Find("Score/Text (TMP)").GetComponent<TextMeshProUGUI>();
            timerTxt = GameObject.Find("Timer/Text (TMP)").GetComponent<TextMeshProUGUI>();
        }
        
        StartCoroutine(WaitforLoad());
        rb.velocity = Vector3.right * 20;

        if (playerPlane == null)
        {
            //Find the player by tag
            playerPlane = GameObject.FindGameObjectWithTag("Player");
        }
        //Find masterAudioMixer and set volume to playerPrefs
        AudioSource src = playerPlane.GetComponent<AudioSource>();
        AudioMixer sfx_Mix = src.outputAudioMixerGroup.audioMixer;
        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2];
        snapshots[0] = sfx_Mix.FindSnapshot("OnDeath");
        snapshots[1] = sfx_Mix.FindSnapshot("Start");
        sfx_Mix.TransitionToSnapshots(snapshots, new float[] {0,1}, 0.5f);

        sfx_Mix.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("Saved_SFX_Volume", 0.5f)) * 20);

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
                int numEnemies = enemySpawner.GetNumEnemies();
                enemySpawner.SpawnEnemy(SpawnPointName.Random, UnityEngine.Random.Range(0, numEnemies));
            }
        }
        levelTimer += Time.deltaTime;
        if (timerTxt != null)
        timerTxt.text = System.TimeSpan.FromSeconds((double)levelTimer).ToString(@"m\:ss");
    }

    private void OnEnable(){
        // Update Score on enemy death 
        EnemyBase.OnEnemyDeath += UpdateScore;
        PlayerPlane.OnPlayerDeath += GameOver;
        PlayerPlane.OnPlayerDamage += PlayDamageEffect;
        BossEnemy.OnBossDeath += TriggerDelayedWin;

    }

    private void OnDisable(){
        // if gameobject is disabled remove all listeners
        EnemyBase.OnEnemyDeath -= UpdateScore;
        PlayerPlane.OnPlayerDeath -= GameOver;
        PlayerPlane.OnPlayerDamage -= PlayDamageEffect;
        BossEnemy.OnBossDeath -= TriggerDelayedWin;

    }

    void TriggerDelayedWin(){
        StartCoroutine(WaitThenWin());
    }

    
    IEnumerator WaitThenWin(){
        yield return new WaitForSeconds(5);
        YouWin();
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            UpdatePerspective(col.GetComponent<TransitionPoint>().GetPerspective());
            if (playerPlane != null){
                playerPlane.GetComponent<Autopilot>().yTarget = col.transform.position.y;
            }
        }
        if(col.CompareTag("WinPoint")){
            Debug.Log("Hit win point");
            YouWin();
        }
    }

    // wait function to call events for objects not active on start / level loaded
    private IEnumerator WaitforLoad(){
        yield return new WaitForSeconds(1);
        UpdatePerspective(currentPerspective);

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
        //Debug.Log("Update Score");
        if(ScoreTxt){
            score += enemy.scoreValue;
            ScoreTxt.text = score.ToString();
        }
    }

    private void PlayDamageEffect(PlayerPlane playerPlane){
        AudioSource src = playerPlane.GetComponent<AudioSource>();
        AudioMixer sfx_Mix = src.outputAudioMixerGroup.audioMixer;
        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2];
        snapshots[0] = sfx_Mix.FindSnapshot("Start");
        snapshots[1] = sfx_Mix.FindSnapshot("OnDeath");

        float invMuffle = Mathf.Clamp01(playerPlane.CurrentHealth / playerPlane.maxHealth);
        float dmgMuffle = 1 - invMuffle;

        sfx_Mix.TransitionToSnapshots(snapshots, new float[] {invMuffle,dmgMuffle}, 0.5f);
        damageAnim.SetTrigger("DamageTaken");
    }

    private void GameOver(){
        //Audio transitions
        AudioListener listener = Camera.main.GetComponent<AudioListener>();
        listener.enabled = true;
        AudioSource src = GetComponent<AudioSource>();
        AudioClip deathSound = src.clip;
        src.PlayOneShot(deathSound);
        AudioMixer sfx_Mix = src.outputAudioMixerGroup.audioMixer;
        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2];
        snapshots[0] = sfx_Mix.FindSnapshot("Start");
        snapshots[1] = sfx_Mix.FindSnapshot("OnDeath");
        sfx_Mix.TransitionToSnapshots(snapshots, new float[] {0,1}, 0.5f);

        //Wreckage
        GameObject wreckage = GameObject.FindWithTag("PlayerWreckage");
        //Pick a random child from player wreckage
        Transform randomChild = wreckage.transform.GetChild(0).GetChild(UnityEngine.Random.Range(0, wreckage.transform.childCount));

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
        StartCoroutine(LerpTime(0, 1.0f));
        //Save this level index as maxLevelComplete
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevelComplete = Mathf.Max(PlayerPrefs.GetInt("maxLevelCompleted", 0), currentSceneIndex);
        PlayerPrefs.SetInt("MaxLevelCompleted", maxLevelComplete);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (levelClearPnl == null){
            gameManager.SetNextScene(nextSceneIndex);
            //Go to the loadout
            gameManager.GoToLevelViaLoadout(nextSceneIndex);
            return;
        }
        SaveScoreTime();
        levelClearPnl.GetComponent<Canvas>().enabled = true;  
        gameManager.SetNextScene(nextSceneIndex);
        
    }

    void EnsureNextLevelButtonListener()
    {
        Button[] buttons = levelClearPnl.GetComponentsInChildren<Button>();
        Debug.Log("Found buttons" + buttons.Length);
        foreach (Button button in buttons)
        {
            Debug.Log("ButtonName: " + button.name);
            if (button.name == "NextLevelButton")
            {
                Button btn = button;
                
                PauseMenu pauseMenu = GetComponent<PauseMenu>();
                Debug.Log("Checking for listeners on NextLevelButton");
                Debug.Log("NextLevelTarget: " + btn.onClick.GetPersistentTarget(0));
                LevelManager lm = GetComponent<LevelManager>();
                btn.onClick.AddListener(lm.GoToNextLevel);
                if (btn.onClick.GetPersistentTarget(0) != lm)
                {
                    Debug.Log("Adding listener to NextLevelButton");
                    btn.onClick.AddListener(lm.GoToNextLevel);
                }
            }
        }
    }
    void SaveScoreTime()
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        string playerName = PlayerPrefs.GetString("PlayerName");
        string scene_player = sceneName + "_" + playerName;
        HighScoreManager.HighScoreEntry highScoreEntry = new()
                                        {
                                            level = sceneName,
                                            name = playerName,
                                            score = score,
                                            time = levelTimer
                                        };
        levelClearPnl.GetComponent<Canvas>().enabled = true;
        levelClearPnl.GetComponent<HighScoreManager>().AddHighScoreEntry(highScoreEntry);
    }

    IEnumerator LerpTime(float finalScale, float lerpPeriod)
    {
        float elapsedTime = 0;
        float startScale = Time.timeScale;

        while (elapsedTime < lerpPeriod)
        {
            Time.timeScale = Mathf.Lerp(startScale, finalScale, elapsedTime / lerpPeriod);
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore time scale
            yield return null;
        }

        Time.timeScale = finalScale;
    }

    public void GoToNextLevel(){
        //Find gm instance

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        gameManager.GetComponent<GameManager>().GoToLevelViaLoadout(nextSceneIndex);
    }

    //Referenced by unityAction onClick in Pause Menu / GameOverMenu
    private void Restart(){
        SceneManager.LoadScene(1);
        Time.timeScale = 1; 
    }

}
