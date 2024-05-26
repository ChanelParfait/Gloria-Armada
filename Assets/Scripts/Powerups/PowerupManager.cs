using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PowerupManager : MonoBehaviour
{
    public GameObject PowerupPrefab;
    public string[] powerupItems; // Array to store the names of powerup items
    [SerializeField] private float dropChance = 5f;

    // HUD Elements
    public Transform powerupIconContainer; // The parent container for powerup icons
    public GameObject powerupIconPrefab; // The prefab for a powerup icon
    private List<GameObject> collectedPowerups = new List<GameObject>();

    void Start()
    {
        // Initialize any necessary components here
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
        if (CheckDrop())
        {
            GameObject powerUp = Instantiate(PowerupPrefab, enemyPosition, Quaternion.identity);
            Rigidbody rb = powerUp.GetComponentInChildren<Rigidbody>();
            rb.AddForce(velocity, ForceMode.VelocityChange);
            // Debug.Log("Power-up spawned at: " + enemyPosition);
        }
    }

    public void AddPowerup(Sprite powerupSprite)
    {
        // Instantiate a new icon
        GameObject newIcon = Instantiate(powerupIconPrefab, powerupIconContainer);

        // Set the sprite of the icon
        Image iconImage = newIcon.GetComponent<Image>();
        iconImage.sprite = powerupSprite;

        // Make the initial image transparent
        Color transparentColor = iconImage.color;
        transparentColor.a = 0; // Set alpha to 0 for full transparency
        iconImage.color = transparentColor;

        // Gradually fade in the image
        StartCoroutine(FadeInImage(iconImage));

        // Add the icon to the list of collected powerups
        collectedPowerups.Add(newIcon);
    }

    private IEnumerator FadeInImage(Image image)
    {
        float duration = 1f; // Duration of the fade-in
        float elapsed = 0f;

        Color color = image.color;
        color.a = 0f; // Start with full transparency
        image.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration); // Gradually increase alpha
            image.color = color;
            yield return null;
        }
    }
}
