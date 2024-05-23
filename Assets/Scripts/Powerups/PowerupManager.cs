using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PowerupManager : MonoBehaviour
{
    public GameObject PowerupPrefab;
    public string[] powerupItems; // Array to store the names of powerup items
    [SerializeField] private float dropChance = 5f;

    Burn burn;

    
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
    public void SpawnPowerUp(Vector3 enemyPosition, Vector3 velocity)
    {

        if(CheckDrop()){
            GameObject powerUp = Instantiate(PowerupPrefab, enemyPosition, Quaternion.identity);
            Rigidbody rb = powerUp.GetComponentInChildren<Rigidbody>();
            rb.AddForce(velocity, ForceMode.VelocityChange);
            //Debug.Log("Power-up spawned at: " + enemyPosition);
        }
     
    }
    
    public IEnumerator BurnDamageOverTime(EnemyPlane enemy, float burnDamage, float burnDuration, float burnTime)
    {
        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            if (enemy != null)
            {
                enemy.maxHealth -= burnDamage;
                Debug.Log("Burn damage applied. Current health: " + enemy.maxHealth);
                yield return new WaitForSeconds(burnTime);
                elapsed += burnTime;
            }
            else
            {
                break; // Stop coroutine if enemy is null (e.g., if it is destroyed)
            }
        }
    }

}