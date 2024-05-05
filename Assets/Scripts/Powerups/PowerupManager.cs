using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PowerupManager : MonoBehaviour
{
    public GameObject PowerupPrefab;
    public string[] powerupItems; // Array to store the names of powerup items
    [SerializeField] private float dropChance = 5f;


    
    void Start()
    {

    }


    public bool CheckDrop()
    {
        // Generate a random number between 0 and 100
        int randomNumber = Random.Range(0, 101);

        // Check if the random number is less than or equal to the drop chance
        if (randomNumber <= dropChance)
        {
            // Drop occurs
            return true;
        }
        else
        {
            // Drop does not occur
            return false;
        }
    }    
    public void SpawnPowerUp(Vector3 enemyPosition)
    {

        if(CheckDrop()){
            Instantiate(PowerupPrefab, enemyPosition, Quaternion.identity);
            //Debug.Log("Power-up spawned at: " + enemyPosition);
        }
     
    }

}