using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager Instance; 
    // Player Values 
    public GameObject PrimaryWeapon;
    public GameObject SpecialWeapon;

    public PlaneStats planeBody;

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
        if (Instance == null)
            Instance = this;
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
            Actor playerPlane = player.GetComponent<Actor>();
            if (playerPlane){
                playerPlane.maxHealth = planeBody.health;
            }
        }

    }

    void OnSceneChanged(Scene scene, LoadSceneMode mode){
        if (SceneManager.GetActiveScene().buildIndex == 4){
            // Set up player
            GameObject player  = GameObject.FindGameObjectWithTag("PlayerWeaponManager");
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
