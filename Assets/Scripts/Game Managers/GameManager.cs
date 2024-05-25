using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    // Player Values 
    public GameObject PrimaryWeapon;
    public GameObject SpecialWeapon;

    public PlaneStats planeBody;

    public string loadoutSceneName = "Loadout"; // Name of your loadout scene
    [SerializeField] string nextLevelSceneName; // Index of the next level scene

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNextScene(int nextSceneIndex){
        string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        nextLevelSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
    }

    IEnumerator ApplyLoadout(){
        yield return new WaitForSeconds(1);
        GameObject playerWeapons  = GameObject.FindGameObjectWithTag("PlayerWeaponManager");
        if (playerWeapons){
            PlayerWeaponManager manager = playerWeapons.GetComponent<PlayerWeaponManager>();
            if (manager){
                manager.SetPrimaryWeapon(PrimaryWeapon);
                manager.SetSpecialWeapon(SpecialWeapon);
                // set body 
            }
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (playerWeapons){
            Plane playerPlaneStats = player.GetComponent<Plane>();
            if (playerPlaneStats){
                playerPlaneStats.surfaces = planeBody.surfaces;
                playerPlaneStats.weight = planeBody.weight;
                playerPlaneStats.thrust = planeBody.thrust;
                playerPlaneStats.thrustVectoring = planeBody.thrustVectoring;
                playerPlaneStats.scaleVelocity = planeBody.scaleVelocity;
                playerPlaneStats.cd = planeBody.cd;
                playerPlaneStats.liftPower = planeBody.liftPower;
            }
            PlayerPlane playerPlane = player.GetComponent<PlayerPlane>();
            if (playerPlane){
                playerPlane.maxHealth = planeBody.health;
                playerPlane.SetHealth(planeBody.health);
            }
        }

    }

    // Method to be called when a level is completed
    public void OnLevelComplete(int nextLevelIndex)
    {
        //If the next index is out of bounds, set nextLevel index to 4 ("Level_1")
        if (SceneManager.sceneCountInBuildSettings < nextLevelIndex)
        {
            Debug.Log("Scenes in buid:" + SceneManager.sceneCountInBuildSettings + " nextLevelIndex:" + nextLevelIndex);
            nextLevelIndex = 4;
            //Load the main menu (index 0)
            SceneManager.LoadScene(0);
            return;
        }
        //If the next scene index is the loadout scene, set the next scene to the next level
        if (nextLevelIndex == 2) nextLevelIndex = 4;
        SetNextScene(nextLevelIndex);
        SceneManager.LoadScene(loadoutSceneName);
    }
    // Method to be called from the loadout scene to proceed to the next level
    public void LoadNextLevel()
    {
        //If loadout scene, load the intro scene
        if (SceneManager.GetActiveScene().buildIndex == 2){
            SceneManager.LoadScene(3);
            return;
        }
        SceneManager.LoadScene(nextLevelSceneName);
    }

    void OnSceneChanged(Scene scene, LoadSceneMode mode){
        if (scene.buildIndex >= 4){
            GameObject player  = GameObject.Find("PlayerPhys/Player_Prefab");
            if (player){
                PlayerWeaponManager manager = player.GetComponent<PlayerWeaponManager>();
                if (manager){
                    manager.SetPrimaryWeapon(PrimaryWeapon);
                    manager.SetSpecialWeapon(SpecialWeapon);
                    // set body 
                }
            }
            StartCoroutine(ApplyLoadout());
        }     
    }
}
