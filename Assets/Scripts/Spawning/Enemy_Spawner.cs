using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    // enemy spawner is parented to the camera for dynamic spawning
    // spawning plane must be parallel and aligned with the player to ensure
    // that enemies are spawned on the same plane and their movement is fixed to that plane
    // if camera position or angle is changed, the spawner / spawn points must be moved to align with the player plane 
    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private Perspective currentPerspective; 
    [SerializeField] private GameObject[] enemies; 

    public bool isEnabled = true;

    [SerializeField] private Camera cam ; 
    [SerializeField] Dictionary<string, GameObject> spawnPoints = new Dictionary<string, GameObject>();

    Vector3 velocity;
    Vector3 lastPosition;


    private void OnEnable(){
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    private void OnDisable(){
        // if gameobject is disabled remove all listeners
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    public int GetNumEnemies(){
        return enemies.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in gameObject.transform){
            // get all spawnpoints and store them in a dict 
            spawnPoints.Add(child.gameObject.name, child.gameObject);
        }
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position; 
    }

    // Spawn a given amount of enemies at a certain spawn point 
    private IEnumerator SpawnEnemies(SpawnPointName spawnPointName, int enemyIndex, int spawnAmount, int spawnInterval){
        for(int i = 0; i < spawnAmount; i++){
            // Spawn enemy 
            SpawnEnemy(spawnPointName, enemyIndex); 
            // then wait 3 seconds before next spawn 
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector2 GetCameraDimensions(){
        //With a perspective cam at -250 units on the z axis, find the bounds of the camera view using the camera's FOV
        float height = 2.0f * Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        float width = height * cam.aspect;  

        return new Vector2(width, height);
    }

    //Spawn an enemy of given type(index) at a given spawn point 
    public void SpawnEnemy(SpawnPointName spawnPointName, int enemyIndex){
        // given a spawn position name, find the corresponding spawn point gameobject from the list of spawn points
        GameObject spawnPoint;
        if (spawnPoints.TryGetValue(spawnPointName.ToString(), out spawnPoint)){
            // check if given enemy index is valid 
            if(enemyIndex < enemies.Length){
                GameObject enemy = enemies[enemyIndex]; 

                //get movement direction and rotation
                Vector3 orientation = enemy.GetComponent<EnemyPlane>().orientation;
                Vector3 moveDir = enemy.GetComponent<EnemyPlane>().moveDir;

                //Spawn position just off screen depending on perspective
                Vector3 spawnCenter = Vector3.zero;
                float spawnSize = 0.0f;
                Vector3 spawnPos = Vector3.zero;
                switch (currentPerspective){
                    case Perspective.Top_Down:
                        spawnCenter = new Vector3(transform.position.x + GetCameraDimensions().y/2 + 40.0f, 0, 0);
                        spawnSize = GetCameraDimensions().x * 0.3f;
                        spawnPos = spawnCenter + new Vector3(0, 0, Random.Range(-spawnSize, spawnSize));
                        //orientation = cameraTransform.up * -1;
                        break;
                    case Perspective.Side_On:
                        spawnCenter = new Vector3(transform.position.x + GetCameraDimensions().x/2 + 40.0f, 0, 0);
                        spawnSize = GetCameraDimensions().y * 0.3f;
                        spawnPos = spawnCenter + new Vector3(0, Random.Range(-spawnSize, spawnSize), 0);
                        //orientation = cameraTransform.right * -1;
                        break;
                }

                

                // spawn enemy as a child of the spawner
                // providing it a relative position and rotation 
                GameObject spawnedEnemy = Instantiate(enemy, spawnPos, Quaternion.LookRotation(orientation, Vector3.up));

                // set the movement direction of the enemy
                EnemyPlane e;
                if ((e = spawnedEnemy.GetComponent<EnemyPlane>()) != null)
                {
                    e.moveDir = moveDir;
                    e.orientation = orientation;
                    e.referenceSpeed = velocity.x;
                }
                else
                {
                    Debug.Log("Enemy does not have Enemy component");
                }
            }  
        }
    }

    private void OnTriggerEnter(Collider col){
        //Debug.Log("Trigger");

        if(col.tag == "SpawnTrigger" && isEnabled){
            // upon colliding with a spawn trigger, retrieve spawn parameters and start the spawning coroutine
            SpawnTrigger trigger = col.GetComponent<SpawnTrigger>();
            StartCoroutine(SpawnEnemies(trigger.spawnPointName, trigger.enemyIndex, trigger.spawnAmount, trigger.spawnInterval));
            // disable this collider so it only executes once
            col.enabled = false; 
        }
    }

    // get the appropriate orientation of the enemy based on its spawn point
    // and which perpectivethe game is in 
    private Vector3 GetOrientation(GameObject spawnPoint){

        if(currentPerspective == Perspective.Top_Down){
            if(spawnPoint.name == "Left_Bottom" || spawnPoint.name == "Right_Bottom"){
                // face toward top of screen
                return cameraTransform.up * -1;
            }
            if(spawnPoint.name == "Left_Top" || spawnPoint.name == "Right_Top"){
                // face toward bottom of screen 
                return cameraTransform.up;
            }
            if(spawnPoint.name == "Top_Right" || spawnPoint.name == "Bottom_Right"){
                // face toward left side of screen 
                return cameraTransform.right;
            }
            if(spawnPoint.name == "Top_Left" || spawnPoint.name == "Bottom_Left"){
                // face toward right side of screen
                return cameraTransform.right  * -1;
            }

        }
        if(currentPerspective == Perspective.Side_On){
            // face towards left side of camera 
            return cameraTransform.right  * 1;
        }
        else
        {
            Debug.Log("Invalid Perspective Given");
            return new Vector3(); 
        }
    }

    // get the appropriate movement direction of the enemy based on its spawn point
    private Vector3 GetMoveDirection(GameObject spawnPoint){

            if(spawnPoint.name == "Top_Left" || spawnPoint.name == "Top_Right"){
                // move toward bottom of screen
                return cameraTransform.up  * -1;
            }
            else if(spawnPoint.name == "Bottom_Left" || spawnPoint.name == "Bottom_Right"){
                // move toward top of screen 
                return cameraTransform.up;
            }
            else if(spawnPoint.name == "Right_Top" || spawnPoint.name == "Right_Bottom"){
                // move toward left side of screen 
                return cameraTransform.right * -1;
            }
            else if(spawnPoint.name == "Left_Top" || spawnPoint.name == "Left_Bottom"){
                // move toward right side of screen
                return cameraTransform.right;
            }
            else{
                return new Vector3();
            }
    }

    public void UpdatePerspective(int newPers){
        currentPerspective = (Perspective)newPers; 
    }
}
