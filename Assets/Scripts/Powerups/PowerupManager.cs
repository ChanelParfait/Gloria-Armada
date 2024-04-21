using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PowerupManager : MonoBehaviour
{
    JetControl jetControl;
    Enemy enemy;
    Projectile proj;
    public string[] powerupItems; // Array to store the names of powerup items
    public Text powerupText; // Reference to the UI Text element for displaying powerup item name

    
    void Start(){

        //Checking for Scripts 
        jetControl = FindObjectOfType<JetControl>();
        if (jetControl == null)
    {
        Debug.LogError("JetControl script not found on the player GameObject!");
    }
     enemy = FindObjectOfType<Enemy>();
        if (enemy == null)
    {
        Debug.LogError("Enemy script not found on the enemy GameObject!");
    }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")) // Check if the colliding object is the player
        {

            // Randomly select a powerup item
            string selectedPowerup = powerupItems[Random.Range(0, powerupItems.Length)];

            // Apply the powerup effect based on the selected item
            ApplyPowerup(selectedPowerup);

            // Display the name of the obtained powerup item on the screen
            powerupText.text = "Powerup Obtained: " + selectedPowerup;

            Destroy(gameObject);

        }
    }

    // Apply powerup effect based on the selected item
    private void ApplyPowerup(string selectedPowerup)
    {
        // Call the appropriate function based on the selected powerup
        switch (selectedPowerup)
        {
            case "Powerup1":
                Powerup1();
                break;
            // case "Powerup2":
            //     Powerup2();
            //     break;
                case "Powerup3":
                Powerup3();
                break;
            case "Powerup4":
                Powerup4();
                break;
                case "Powerup5":
                Powerup5();
                break;
            // Add more cases for other powerups
            default:
                Debug.LogWarning("Unknown powerup: " + selectedPowerup);
                break;
        }
    }

      IEnumerator ApplyTickDamage(Enemy enemy)
        {
            Debug.Log("Worky Work");
            float damageAmount = 0.5f;
            float duration = 3f;
            float elapsedTime = 0f;

            // Loop for the duration of the effect
            while (elapsedTime < duration)
            {
                Debug.Log("WRRRRRR");

                // Apply damage to enemies here
                // For demonstration, assuming there's an 'enemy' variable available
                if (enemy.hit == true)
                {
                    enemy.health -= damageAmount;
                }

                // Wait for the next tick
                yield return new WaitForSeconds(1f); // Wait for 1 second before applying damage again
                elapsedTime += 1f; // Increment elapsed time
            }
        }

    // Function for Powerup1
    private void Powerup1()
    {
        // Implement the effect of Powerup1
        Debug.Log("Fire Rate Up!");
        jetControl.shootDelay -= 0.05f;
    }

    // Function for Powerup2
    // private void Powerup2()
    // {
    //     // Implement the effect of Powerup2
    //     Debug.Log("Fire Damage!");
    //     StartCoroutine(ApplyTickDamage(enemy));
        
    // }


    private void Powerup3()
    {
        // Implement the effect of Powerup2
        Debug.Log("Damage up!");
        enemy.incomingDamage +=1f; 

    }

    private void Powerup4()
    {
        // Implement the effect of Powerup2
        Debug.Log("Shot Size up!");
        jetControl.projectile.transform.localScale *= 1.5f;

    }

      private void Powerup5()
    {
        // Implement the effect of Powerup2
        Debug.Log("Shot Speed up!");
        proj.speed +=5;

    }
    // Add more functions for other powerups as needed
}