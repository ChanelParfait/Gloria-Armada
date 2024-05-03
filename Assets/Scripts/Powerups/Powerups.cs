using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PowerupManager powerupManager; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")) // Check if the colliding object is the player
        {

            // Randomly select a powerup item
           // string selectedPowerup = powerupItems[Random.Range(0, powerupItems.Length)];

            // Apply the powerup effect based on the selected item
            //ApplyPowerup(selectedPowerup);

            // Display the name of the obtained powerup item on the screen
            // Debug.Log("Collision Detected");
            Destroy(gameObject);

        }
    }
}
