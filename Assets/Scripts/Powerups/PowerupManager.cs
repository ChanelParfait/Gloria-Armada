using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PowerupManager : MonoBehaviour
{
    public GameObject PowerupPrefab;
    public string[] powerupItems; // Array to store the names of powerup items
    [SerializeField] private float dropChance = 5f;

    
    void Start(){

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
            Debug.Log("Power-up spawned at: ");
        }
     
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")) // Check if the colliding object is the player
        {

            // Randomly select a powerup item
            string selectedPowerup = powerupItems[Random.Range(0, powerupItems.Length)];

            // Apply the powerup effect based on the selected item
            //ApplyPowerup(selectedPowerup);

            // Display the name of the obtained powerup item on the screen

            Destroy(gameObject);

        }
    }

}